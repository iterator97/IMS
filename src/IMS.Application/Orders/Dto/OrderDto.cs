using System;

namespace IMS.Application.Orders.Dto
{
    public record OrderDto
    {
        public Guid ProductId { get; init; }
        public int Quantity { get; init; }
    }
}
