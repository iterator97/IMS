using IMS.Domain.Discounts;
using IMS.Domain.Products;
using IMS.Domain.Users;

namespace IMS.UnitTests.Mock
{
    public static class MockData
    {
        public static List<User> GetUsers()
        {
            return
            [
                new()
                {
                    Id = Guid.Parse("b22d4436-5576-4000-bcaf-053cbbe91941"),
                    Name = "Anna",
                    Surname = "Kowalska",
                    Email = "anna.kowalska@example.com"
                },
                new()
                {
                    Id = Guid.Parse("65b8a976-f2df-42d0-a91a-c95af6fe3bd8"),
                    Name = "John",
                    Surname = "Duen",
                    Email = "john.duen@example.com"
                }
            ];
        }

        public static List<Address> GetAddresses(IReadOnlyList<User> users)
        {
            return
            [
                new()
                {
                    Id = Guid.Parse("7f5dc35f-7e7a-4a9b-8501-2f5acc669527"),
                    UserId = users[0].Id,
                    Street = "Dluga",
                    Number = "12A",
                    PostalCode = "00-001",
                    City = "Warsaw",
                    Country = "Poland",
                    Region = "Europe"
                },
                new()
                {
                    Id = Guid.Parse("d8f3877e-ed3f-4ee4-a4cf-53572b8145ee"),
                    UserId = users[1].Id,
                    Street = "Sample Street",
                    Number = "2",
                    PostalCode = "90-102",
                    City = "Tokyo",
                    Country = "Japan",
                    Region = "Asia"
                },
                new()
                {
                    Id = Guid.Parse("d8f3877e-ed3f-4ee4-a4cf-53572b8145ee"),
                    UserId = users[1].Id,
                    Street = "Sample US Street",
                    Number = "2",
                    PostalCode = "92-200",
                    City = "London",
                    Country = "England",
                    Region = "US"
                }
            ];
        }

        public static List<Product> GetProducts()
        {
            return [
                    new()
                {
                    Id = Guid.Parse("98e19074-75b0-4a7b-a8b2-281fc8924c0e"),
                    Name = "Laptop Lenovo ThinkPad",
                    Description = "Laptop Intel i5, RAM 16 GB, SSD 512 GB.",
                    Price = 4299.99m,
                    Stock = 10
                },
                new()
                {
                    Id = Guid.Parse("bb52ea10-7943-4d4e-9f86-154a1fd860e0"),
                    Name = "Monitor Dell 27",
                    Description = "Monitor QHD IPS z regulacja wysokosci.",
                    Price = 1299.00m,
                    Stock = 15
                },
                new()
                {
                    Id = Guid.Parse("3356eb68-3c46-409f-8986-9d78b6ca1958"),
                    Name = "Klawiatura Logitech",
                    Description = "Klawiatura mechaniczna RGB.",
                    Price = 349.99m,
                    Stock = 30
                },
        ];
        }

        public static List<Discount> GetDiscounts()
        {
            return [ new Discount
                {
                    Id = Guid.NewGuid(),
                    Country = "Polska",
                    Region = "Europe",
                    Amount = 0.4m,
                    Enabled = true,
                    Mode = DiscountMode.OnAll,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1)
                },
                new Discount
                {
                    Id = Guid.NewGuid(),
                    Country = "Polska",
                    Region = "Europe",
                    Amount = 0.4m,
                    Enabled = true,
                    Mode = DiscountMode.OnHighest,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1)
                }

            ];
        }
    }
}
