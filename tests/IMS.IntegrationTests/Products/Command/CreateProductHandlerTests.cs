using IMS.Application.Products.Command;
using IMS.Application.Products.Dto;
using IMS.IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace IMS.IntegrationTests.Products.Command;

[Collection(IntegrationTestCollection.Name)]
public sealed class CreateProductHandlerTests(PostgresIntegrationTestFixture fixture)
{
    [Fact]
    public async Task Handle_WhenProductIsValid_ShouldSaveProductAndReturnGeneratedId()
    {
        // Arrange
        await fixture.ResetDatabaseAsync();
        await using var context = fixture.CreateContext();
        var handler = new CreateProduct.Handler(context, NullLogger<CreateProduct.Handler>.Instance);

        // Act
        var result = await handler.Handle(
            new CreateProduct.Command
            {
                ProductDto = new CreateProductDto
                {
                    Name = "Test product",
                    Description = "Test description",
                    Price = 100,
                    Stock = 7
                }
            },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value);

        var savedProduct = await context.Products.SingleAsync();

        Assert.Equal(result.Value, savedProduct.Id);
        Assert.Equal("Test product", savedProduct.Name);
        Assert.Equal("Test description", savedProduct.Description);
        Assert.Equal(100, savedProduct.Price);
        Assert.Equal(7, savedProduct.Stock);
    }
}
