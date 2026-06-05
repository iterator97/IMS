using IMS.Application.Orders.Command;
using IMS.Application.Orders.Dto;
using IMS.Application.Services;
using IMS.IntegrationTests.Fixtures;
using IMS.IntegrationTests.Mock;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace IMS.IntegrationTests.Orders.Command;

[Collection(IntegrationTestCollection.Name)]
public sealed class CreateOrderHandlerTests(PostgresIntegrationTestFixture fixture)
{
    [Fact]
    public async Task Handle_WhenOrderIsValid_ShouldSaveOrderAndReturnGeneratedId()
    {
        // Arrange
        await fixture.ResetAndSeedAsync();

        await using var context = fixture.CreateContext();
        var orderCalculator = new OrderCalculator();

        var handler = new CreateOrder.Handler(
            context,
            NullLogger<CreateOrder.Handler>.Instance,
            orderCalculator);

        // Act
        var result = await handler.Handle(
            new CreateOrder.Command
            {
                Order = new CreateOrderDto
                {
                    UserId = MockData.TestUsers()[0].Id,
                    AddressId = MockData.TestAddresses()[0].Id,
                    Orders =
                    [
                        new OrderDto { ProductId = MockData.TestProducts()[0].Id, Quantity = 2 },
                        new OrderDto { ProductId = MockData.TestProducts()[1].Id, Quantity = 10 },
                    ]
                }
            },
            CancellationToken.None);

        var savedOrder = await context.Orders
            .Include(order => order.Items)
            .SingleAsync(order => order.Id == result.Value);

        var firstProduct = await context.Products.SingleAsync(product => product.Id == MockData.TestProducts()[0].Id);
        var secondProduct = await context.Products.SingleAsync(product => product.Id == MockData.TestProducts()[1].Id);

        var firstItem = savedOrder.Items.Single(item => item.ProductId == MockData.TestProducts()[0].Id);
        var secondItem = savedOrder.Items.Single(item => item.ProductId == MockData.TestProducts()[1].Id);

        // Assert
        Assert.True(result.IsSuccess);

        Assert.Equal(0.15m, savedOrder.LocationCharge);
        Assert.Equal(20357.28m, savedOrder.TotalAmount);

        Assert.Equal(2, savedOrder.Items.Count);

        Assert.Equal(2, firstItem.Quantity);
        Assert.Equal(0.15m, firstItem.Discount);

        Assert.Equal(10, secondItem.Quantity);
        Assert.Equal(0.2m, secondItem.Discount);
        
        Assert.Equal(8, firstProduct.Stock);
        Assert.Equal(5, secondProduct.Stock);
    }


    [Fact]
    public async Task Handle_WhenInsufficientStock_ShouldNotSaveOrder()
    {
        // Arrange
        await fixture.ResetAndSeedAsync();

        await using var context = fixture.CreateContext();
        var orderCalculator = new OrderCalculator();

        var handler = new CreateOrder.Handler(
            context,
            NullLogger<CreateOrder.Handler>.Instance,
            orderCalculator);

        // Act
        var result = await handler.Handle(
            new CreateOrder.Command
            {
                Order = new CreateOrderDto
                {
                    UserId = MockData.TestUsers()[0].Id,
                    AddressId = MockData.TestAddresses()[0].Id,
                    Orders =
                    [
                        new OrderDto { ProductId = MockData.TestProducts()[0].Id, Quantity = 2000 },
                    ]
                }
            },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_WhenTwoOrdersArePlacedAtTheSameTime_ShouldSaveOnlyOneOrder()
    {
        // Arrange
        await fixture.ResetAndSeedAsync();

        await using var contextA = fixture.CreateContext();
        await using var contextB = fixture.CreateContext();
        await using var contextC = fixture.CreateContext();

        var handlerA = new CreateOrder.Handler(
            contextA,
            NullLogger<CreateOrder.Handler>.Instance,
            new OrderCalculator());

        var handlerB = new CreateOrder.Handler(
            contextB,
            NullLogger<CreateOrder.Handler>.Instance,
            new OrderCalculator());

        // Act
        var results = await Task.WhenAll(
            handlerA.Handle(CreateOrderCommand(MockData.TestProducts()[0].Id, quantity: 6), CancellationToken.None),
            handlerB.Handle(CreateOrderCommand(MockData.TestProducts()[0].Id, quantity: 6), CancellationToken.None));

        var orderedProduct = await contextC.Products.SingleAsync(product => product.Id == MockData.TestProducts()[0].Id);
        var ordersCount = await contextC.Orders.CountAsync();

        // Assert
        Assert.Equal(1, results.Count(result => result.IsSuccess));
        Assert.Equal(1, results.Count(result => result.IsError));
        Assert.Equal(4, orderedProduct.Stock);
        Assert.Equal(1, ordersCount);
    }

    private static CreateOrder.Command CreateOrderCommand(Guid productId, int quantity)
    {
        return new CreateOrder.Command
        {
            Order = new CreateOrderDto
            {
                UserId = MockData.TestUsers()[0].Id,
                AddressId = MockData.TestAddresses()[0].Id,
                Orders =
                [
                    new OrderDto { ProductId = productId, Quantity = quantity }
                ]
            }
        };
    }
}
