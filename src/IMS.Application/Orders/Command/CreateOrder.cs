using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IMS.Application.Orders.Dto;
using IMS.Application.Services;
using IMS.Application.Shared;
using IMS.Application.Wrappers;
using IMS.Domain.Discounts;
using IMS.Domain.Orders;
using IMS.Domain.Products;
using IMS.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Orders.Command
{
    public static class CreateOrder
    {
        public sealed record Command : IRequest<Result<Guid>>
        {
            public required CreateOrderDto Order { get; init; }
        }

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Order)
                    .NotNull();

                When(x => x.Order != null, () =>
                {
                    RuleFor(x => x.Order.UserId)
                        .NotEmpty();

                    RuleFor(x => x.Order.AddressId)
                        .NotEmpty();

                    RuleFor(x => x.Order.Orders)
                        .NotEmpty();

                    RuleForEach(x => x.Order.Orders)
                        .ChildRules(order =>
                        {
                            order.RuleFor(x => x.ProductId)
                                .NotEmpty();

                            order.RuleFor(x => x.Quantity)
                                .GreaterThan(0);
                        });
                });
            }
        }

        public sealed class Handler(IAppDbContext context, ILogger<Handler> logger, IOrderCalculator orderCalculator)
            : IRequestHandler<Command, Result<Guid>>
        {
            private sealed record ProductQuantity(Guid ProductId, int Quantity);

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!await UserExists(request.Order.UserId, cancellationToken))
                    return Result<Guid>.Error("User was not found.", 404);

                var address = await GetAddress(
                    request.Order.AddressId,
                    request.Order.UserId,
                    cancellationToken);

                if (address == null)
                    return Result<Guid>.Error("Address was not found.", 404);

                var productQuantities = GetProductQuantities(request.Order.Orders);

                var products = await GetProducts(
                    productQuantities,
                    cancellationToken);

                if (products.Count != productQuantities.Count)
                    return Result<Guid>.Error("One or more products were not found.", 404);

                var productsById = products.ToDictionary(product => product.Id);

                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    if (!await DecreaseProductsStock(productQuantities, cancellationToken))
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Guid>.Error("One or more products are out of stock.", 400);
                    }

                    var order = CreateOrderEntity(request.Order, productQuantities);

                    var discount = await GetActiveDiscount(address, order.CreatedAt, cancellationToken);

                    order = orderCalculator.Calculate(
                        order,
                        address.Region,
                        productsById,
                        discount);

                    context.Orders.Add(order);

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    logger.LogInformation(
                        "CreateOrder command executed. Order Id: {OrderId}, Success: {Success}",
                        order.Id,
                        true);

                    return Result<Guid>.Success(order.Id);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);

                    throw;
                }
            }

            private async Task<bool> UserExists(
                Guid userId,
                CancellationToken cancellationToken)
            {
                return await context.Users
                    .AsNoTracking()
                    .AnyAsync(user => user.Id == userId, cancellationToken);
            }

            private async Task<Address?> GetAddress(
                Guid addressId,
                Guid userId,
                CancellationToken cancellationToken)
            {
                return await context.Addresses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        address =>
                            address.Id == addressId &&
                            address.UserId == userId,
                        cancellationToken);
            }

            private async Task<List<Product>> GetProducts(
                IReadOnlyList<ProductQuantity> productQuantities,
                CancellationToken cancellationToken)
            {
                var requestedProductIds = productQuantities
                    .Select(productQuantity => productQuantity.ProductId)
                    .ToList();

                return await context.Products
                    .AsNoTracking()
                    .Where(product => requestedProductIds.Contains(product.Id))
                    .ToListAsync(cancellationToken);
            }

            private static List<ProductQuantity> GetProductQuantities(IReadOnlyList<OrderDto> orderItems)
            {
                return orderItems
                    .GroupBy(order => order.ProductId)
                    .Select(group => new ProductQuantity(
                        group.Key,
                        group.Sum(order => order.Quantity)))
                    .OrderBy(productQuantity => productQuantity.ProductId)
                    .ToList();
            }

            private async Task<bool> DecreaseProductsStock(
                IReadOnlyList<ProductQuantity> productQuantities,
                CancellationToken cancellationToken)
            {
                foreach (var productQuantity in productQuantities)
                {
                    if (await context.Products
                        .Where(product =>
                            product.Id == productQuantity.ProductId &&
                            product.Stock >= productQuantity.Quantity)
                        .ExecuteUpdateAsync(
                            x => x.SetProperty(
                                product => product.Stock,
                                product => product.Stock - productQuantity.Quantity),
                            cancellationToken) == 0)
                        return false;
                }

                return true;
            }

            private static Order CreateOrderEntity(
                CreateOrderDto orderDto,
                IReadOnlyList<ProductQuantity> productQuantities)
            {
                return new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = orderDto.UserId,
                    AddressId = orderDto.AddressId,
                    CreatedAt = DateTime.UtcNow,
                    Items = [.. productQuantities
                        .Select(item => new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        })]
                };
            }

            private async Task<Discount?> GetActiveDiscount(
                Address address,
                DateTime orderDate,
                CancellationToken cancellationToken)
            {
                return await context.Discounts
                    .AsNoTracking()
                    .Where(discount =>
                        discount.Enabled &&
                        discount.StartDate <= orderDate &&
                        discount.EndDate >= orderDate &&
                        discount.Country.ToLower() == address.Country.ToLowerInvariant() &&
                        discount.Region.ToLower() == address.Region.ToLowerInvariant())
                    .OrderByDescending(discount => discount.Amount)
                    .ThenByDescending(discount => discount.StartDate)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }
    }
}
