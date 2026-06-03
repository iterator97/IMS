using System;
using System.Collections.Generic;

namespace IMS.Application.Orders.Dto
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }
        public Guid AddressId { get; set; }
        public IReadOnlyList<OrderDto> Orders { get; set; }
    }
}
