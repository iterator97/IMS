using System;
using System.Collections.Generic;

namespace IMS.Domain;

public class Order
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid AddressId { get; set; }
    public Address Address { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}
