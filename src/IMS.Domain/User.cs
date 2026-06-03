using System;
using System.Collections.Generic;

namespace IMS.Domain;

public class User
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }
    public List<Address> Addresses { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
}
