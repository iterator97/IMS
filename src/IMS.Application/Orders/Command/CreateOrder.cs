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
            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!await UserExistsAsync(request.Order.UserId, cancellationToken))
                    return Result<Guid>.Error("User was not found.", 404);

                if (!await AddressBelongsToUserAsync(
                        request.Order.AddressId,
                        request.Order.UserId,
                        cancellationToken))
                    return Result<Guid>.Error("Address was not found.", 404);

                if (await GetMissingProductIdsAsync(
                    request.Order.Orders,
                    cancellationToken) > 0)
                    return Result<Guid>.Error("One or more products were not found.", 404);

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = request.Order.UserId,
                    AddressId = request.Order.AddressId,
                    CreatedAt = DateTime.UtcNow,
                    Items = request.Order.Orders
                        .Select(item => new OrderItem
                        {
                            Id = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            Quantity = item.Quantity
                        })
                        .ToList()
                };

                context.Orders.Add(order);

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                logger.LogInformation(
                    "CreateOrder command executed. Order ID: {OrderId}, Success: {Success}",
                    order.Id,
                    result);

                return result
                    ? Result<Guid>.Success(order.Id)
                    : Result<Guid>.Error("Failed to create order.");
            }

            private async Task<bool> UserExistsAsync(
                Guid userId,
                CancellationToken cancellationToken)
            {
                return await context.Users
                    .AnyAsync(user => user.Id == userId, cancellationToken);
            }

            private async Task<bool> AddressBelongsToUserAsync(
                Guid addressId,
                Guid userId,
                CancellationToken cancellationToken)
            {
                return await context.Addresses
                    .AnyAsync(
                        address =>
                            address.Id == addressId &&
                            address.UserId == userId,
                        cancellationToken);
            }

            private async Task<int> GetMissingProductIdsAsync(
                IReadOnlyList<OrderDto> orderItems,
                CancellationToken cancellationToken)
            {
                var requestedProductIds = orderItems
                    .Select(order => order.ProductId)
                    .Distinct()
                    .ToList();

                var existingProductIds = await context.Products
                    .Where(product => requestedProductIds.Contains(product.Id))
                    .Select(product => product.Id)
                    .ToListAsync(cancellationToken);

                return requestedProductIds
                    .Except(existingProductIds)
                    .Count();
            }
        }
    }
}
