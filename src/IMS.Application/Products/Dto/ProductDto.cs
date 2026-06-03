using System;

namespace IMS.Application.Products.Dto
{
    public sealed record ProductDto
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required decimal Price { get; init; }
        public required int Stock { get; init; }
    }
}
