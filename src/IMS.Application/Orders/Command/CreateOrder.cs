using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IMS.Application.Orders.Dto;
using IMS.Application.Shared;
using IMS.Application.Wrappers;
using IMS.Domain;
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

        public sealed class Handler(IAppDbContext context, ILogger<Handler> logger)
            : IRequestHandler<Command, Result<Guid>>
        {
            private sealed record ProductQuantity(Guid ProductId, int Quantity);

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await GetUserAsync(request.Order.UserId, cancellationToken);

                if (user == null)
                    return Result<Guid>.Error("User was not found.", 404);

                var address = await GetAddressAsync(
                    request.Order.AddressId,
                    cancellationToken);

                if (address == null)
                    return Result<Guid>.Error("Address was not found.", 404);

                var productQuantities = request.Order.Orders
                    .GroupBy(order => order.ProductId)
                    .Select(group => new ProductQuantity(
                        group.Key,
                        group.Sum(order => order.Quantity)))
                    .OrderBy(productQuantity => productQuantity.ProductId)
                    .ToList();

                var missingProductIds = await GetMissingProductIdsAsync(
                    productQuantities,
                    cancellationToken);

                if (missingProductIds.Count > 0)
                    return Result<Guid>.Error("One or more products were not found.", 404);

                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    if (!await TryDecreaseProductsStockAsync(productQuantities, cancellationToken))
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Guid>.Error("One or more products are out of stock.", 400);
                    }

                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        UserId = request.Order.UserId,
                        AddressId = request.Order.AddressId,
                        CreatedAt = DateTime.UtcNow,
                        Items = [.. request.Order.Orders
                        .Select(item => new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        })]
                    };

                    context.Orders.Add(order);

                    await context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);

                    logger.LogInformation(
                        "CreateOrder command executed. Order ID: {OrderId}, Success: {Success}",
                        order.Id,
                        true);

                    return Result<Guid>.Success(order.Id);
                }
                catch (Exception exception)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    logger.LogError(
                        exception,
                        "CreateOrder command failed for UserId: {UserId}",
                        request.Order.UserId);

                    return Result<Guid>.Error("Failed to create order.");
                }
            }

            private async Task<User> GetUserAsync(
                Guid userId,
                CancellationToken cancellationToken)
            {
                return await context.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
            }

            private async Task<Address> GetAddressAsync(
                Guid addressId,
                CancellationToken cancellationToken)
            {
                return await context.Addresses
                    .FirstOrDefaultAsync(
                        address =>
                            address.Id == addressId,
                        cancellationToken);
            }

            private async Task<List<Guid>> GetMissingProductIdsAsync(
                IReadOnlyList<ProductQuantity> productQuantities,
                CancellationToken cancellationToken)
            {
                var requestedProductIds = productQuantities
                    .Select(productQuantity => productQuantity.ProductId)
                    .ToList();

                var existingProductIds = await context.Products
                    .Where(product => requestedProductIds.Contains(product.Id))
                    .Select(product => product.Id)
                    .ToListAsync(cancellationToken);

                return [.. requestedProductIds.Except(existingProductIds)];
            }

            private async Task<bool> TryDecreaseProductsStockAsync(
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
        }
    }
}
