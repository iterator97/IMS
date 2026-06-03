using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IMS.Application.Orders.Dto;
using IMS.Application.Shared;
using IMS.Application.Wrappers;
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
                var user = await context.Users.FindAsync([request.Order.UserId], cancellationToken);

                if (user == null)
                    return Result<Guid>.Error("User was not found.", 404);

                var addressExists = await context.Addresses
                    .AnyAsync(
                        address =>
                            address.Id == request.Order.AddressId &&
                            address.UserId == request.Order.UserId,
                        cancellationToken);

                if (!addressExists)
                    return Result<Guid>.Error("Address was not found.", 404);

                var requestedProductIds = request.Order.Orders
                    .Select(order => order.ProductId)
                    .Distinct()
                    .ToList();

                var existingProductIds = await context.Products
                    .Where(product => requestedProductIds.Contains(product.Id))
                    .Select(product => product.Id)
                    .ToListAsync(cancellationToken);

                var missingProductIds = requestedProductIds
                    .Except(existingProductIds)
                    .ToList();

                if (missingProductIds.Count > 0)
                    return Result<Guid>.Error("One or more products were not found.", 404);

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                return result
                    ? Result<Guid>.Success(Guid.NewGuid())
                    : Result<Guid>.Error("Failed to create order.");
            }
        }
    }
}
