using System;
using System.Collections.Generic;
using IMS.Domain.Orders;

namespace IMS.Domain.Users;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public List<Address> Addresses { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
}
