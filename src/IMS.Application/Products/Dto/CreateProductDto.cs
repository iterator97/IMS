namespace IMS.Application.Products.Dto
{
    public sealed record CreateProductDto
    {
        public string? Name { get; init; }
        public string? Description { get; init; }
        public decimal? Price { get; init; }
        public int? Stock { get; init; }
    }
}
