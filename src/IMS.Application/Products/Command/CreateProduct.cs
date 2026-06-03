using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using IMS.Application.Products.Dto;
using IMS.Application.Shared;
using IMS.Application.Wrappers;
using IMS.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Products.Command
{
    public static class CreateProduct
    {
        public sealed record Command : IRequest<Result<Guid>>
        {
            public required CreateProductDto ProductDto { get; init; }
        }

        public sealed class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.ProductDto)
                    .NotNull();

                When(x => x.ProductDto != null, () =>
                {
                    RuleFor(x => x.ProductDto.Name)
                        .NotEmpty()
                        .MaximumLength(50);

                    RuleFor(x => x.ProductDto.Description)
                        .NotEmpty()
                        .MaximumLength(50);

                    RuleFor(x => x.ProductDto.Price)
                        .NotNull()
                        .GreaterThan(0);

                    RuleFor(x => x.ProductDto.Stock)
                        .NotNull()
                        .GreaterThanOrEqualTo(0);
                });
            }
        }

        public sealed class Handler(IAppDbContext context, ILogger<Handler> logger) : IRequestHandler<Command, Result<Guid>>
        {
            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.ProductDto.Name!,
                    Description = request.ProductDto.Description!,
                    Price = request.ProductDto.Price!.Value,
                    Stock = request.ProductDto.Stock!.Value
                };

                context.Products.Add(product);

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                logger.LogInformation("CreateProduct command executed. Product ID: {ProductId}, Success: {Success}", product.Id, result);

                return result
                    ? Result<Guid>.Success(product.Id)
                    : Result<Guid>.Error("Failed to create product.");
            }
        }
    }
}
