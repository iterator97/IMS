using System;

namespace IMS.Domain
{
    public class Product
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public required int Stock { get; set; } = 0;
    }
}
