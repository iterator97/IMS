using IMS.Domain.Discounts;
using IMS.Domain.Products;
using IMS.Domain.Users;

namespace IMS.IntegrationTests.Mock
{
    public static class MockData
    {
        public static User[] TestUsers()
        {
            return
                [
                    new User
                    {
                    Id = Guid.Parse("7a5bef8e-e8cf-493d-b291-4d890e87ea12"),
                    Name = "Jan",
                    Surname = "Kowalski",
                    Email = "jan.kowalski@test.local"
                    }
                ];
        }

        public static Address[] TestAddresses()
        {
            return
                [
                new Address
                {
                    Id = Guid.Parse("e1e370a7-5e51-444e-99ee-7163a07ffa8b"),
                    UserId = Guid.Parse("7a5bef8e-e8cf-493d-b291-4d890e87ea12"),
                    Street = "Testowa",
                    Number = "1",
                    PostalCode = "00-001",
                    City = "Warszawa",
                    Country = "Polska",
                    Region = "Europe"
                }
                ];
        }


        public static Discount[] TestDiscounts()
        {
            return
            [
                new Discount
                {
                    Id = Guid.Parse("c40b4821-afb1-46d1-80de-208910177fdc"),
                    Country = "Polska",
                    Region = "Europe",
                    Amount = 0.15m,
                    Enabled = true,
                    Mode = DiscountMode.OnAll,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    EndDate = DateTime.UtcNow.AddDays(1)
                }
            ];
        }

        public static Product[] TestProducts()
        {
            return
            [
                new Product
            {
                Id = Guid.Parse("e465e720-12b0-42c0-a173-9afe90a99ca9"),
                Name = "Laptop Lenovo ThinkPad",
                Description = "Laptop biznesowy Intel i5.",
                Price = 4299.99m,
                Stock = 10
            },
            new Product
            {
                Id = Guid.Parse("cbe17c17-8738-4e4f-8486-749eb8a7f67a"),
                Name = "Monitor Dell",
                Description = "Monitor QHD IPS.",
                Price = 1299.00m,
                Stock = 15
            },
            new Product
            {
                Id = Guid.Parse("711e7590-0706-4a69-bf16-1d363beca4dd"),
                Name = "Klawiatura Logitech",
                Description = "Klawiatura mechaniczna.",
                Price = 349.99m,
                Stock = 30
            },
            new Product
            {
                Id = Guid.Parse("4d699b0b-aba1-4046-9919-3e659ca0f6f6"),
                Name = "Mysz Logitech",
                Description = "Mysz do pracy.",
                Price = 429.99m,
                Stock = 25
            },
            new Product
            {
                Id = Guid.Parse("8c1713f4-72ae-410f-b99a-1d304f93d59f"),
                Name = "Sluchawki Sony",
                Description = "Sluchawki bezprzewodowe.",
                Price = 1499.99m,
                Stock = 8
            }
            ];
        }
    }
}
