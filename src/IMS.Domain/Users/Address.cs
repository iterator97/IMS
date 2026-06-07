using System;

namespace IMS.Domain.Users;

public class Address
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public required string Street { get; set; }
    public required string Number { get; set; }
    public required string PostalCode { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public required string Region { get; set; }
}
