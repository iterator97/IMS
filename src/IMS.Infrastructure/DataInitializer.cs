using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMS.Domain;

namespace IMS.Infrastructure
{
    public class DbInitializer
    {
        public static async Task SeedData(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(GetProducts());
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(GetUsers());
                await context.SaveChangesAsync();
            }

            if (!context.Discounts.Any())
            {
                context.Discounts.AddRange(GetDiscounts());
                await context.SaveChangesAsync();
            }

            if (!context.Addresses.Any())
            {
                var users = context.Users
                    .OrderBy(user => user.Email)
                    .Take(3)
                    .ToList();

                if (users.Count >= 3)
                {
                    context.Addresses.AddRange(GetAddresses(users[0].Id, users[1].Id, users[2].Id));
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Orders.Any())
            {
                var users = context.Users
                    .OrderBy(user => user.Email)
                    .Take(2)
                    .ToList();

                var addresses = context.Addresses
                    .OrderBy(address => address.City)
                    .Take(2)
                    .ToList();

                var products = context.Products
                    .OrderBy(product => product.Name)
                    .Take(4)
                    .ToList();

                if (users.Count >= 3 && addresses.Count >= 3 && products.Count >= 8)
                {
                    context.Orders.AddRange(GetOrders(users, addresses, products));
                    await context.SaveChangesAsync();
                }
            }
        }

        private static List<Product> GetProducts()
        {
            return
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Laptop Lenovo ThinkPad",
                    Description = "Laptop Intel i5, RAM 16 GB, SSD 512 GB.",
                    Price = 4299.99m,
                    Stock = 10
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Monitor Dell 27",
                    Description = "Monitor QHD IPS z regulacja wysokosci.",
                    Price = 1299.00m,
                    Stock = 15
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Klawiatura Logitech",
                    Description = "Klawiatura mechaniczna RGB.",
                    Price = 349.99m,
                    Stock = 30
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Mysz Logitech MX Master 3S",
                    Description = "Ergonomiczna mysz do pracy biurowej.",
                    Price = 429.99m,
                    Stock = 25
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Sluchawki Sony WH-1000XM5",
                    Description = "Sluchawki z redukcja szumow.",
                    Price = 1499.99m,
                    Stock = 8
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Dysk SSD Samsung 1TB",
                    Description = "Szybki dysk SSD NVMe o pojemnosci 1 TB.",
                    Price = 399.99m,
                    Stock = 40
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Smartfon Samsung Galaxy",
                    Description = "Smartfon AMOLED, 128 GB pamieci.",
                    Price = 2499.00m,
                    Stock = 12
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Tablet Apple iPad",
                    Description = "Tablet Retina, 64 GB pamieci.",
                    Price = 1999.99m,
                    Stock = 6
                }
            ];
        }

        private static List<User> GetUsers()
        {
            return
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Anna",
                    Surname = "Kowalska",
                    Email = "anna.kowalska@example.com"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Jan",
                    Surname = "Nowak",
                    Email = "jan.nowak@example.com"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "John",
                    Surname = "Duen",
                    Email = "john.duen@example.com"
                }
            ];
        }

        private static List<Address> GetAddresses(Guid firstUserId, Guid secondUserId, Guid thirdUserId)
        {
            return
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = firstUserId,
                    Street = "Dluga",
                    Number = "12A",
                    PostalCode = "00-001",
                    City = "Warsaw",
                    Country = "Poland",
                    Continent = "Europe"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = secondUserId,
                    Street = "Krotka",
                    Number = "8",
                    PostalCode = "30-002",
                    City = "Cracow",
                    Country = "Poland",
                    Continent = "Europe"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = thirdUserId,
                    Street = "Sample",
                    Number = "2",
                    PostalCode = "90-102",
                    City = "Tokyo",
                    Country = "Japan",
                    Continent = "Asia"
                }

            ];
        }

        private static List<Discount> GetDiscounts()
        {
            return
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    Country = "Poland",
                    Continent = "Europe",
                    Amount = 0.15m,
                    Enabled = true,
                    Mode = DiscountMode.OnAll,
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = DateTime.UtcNow.AddMonths(1)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Country = "Japan",
                    Continent = "Asia",
                    Amount = 0.2m,
                    Enabled = true,
                    Mode = DiscountMode.OnAll,
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = DateTime.UtcNow.AddMonths(1)
                }
            ];
        }

        private static List<Order> GetOrders(
            IReadOnlyList<User> users,
            IReadOnlyList<Address> addresses,
            IReadOnlyList<Product> products)
        {
            return
            [
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = users[0].Id,
                    AddressId = addresses[0].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    Items =
                    [
                        new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = products[0].Id,
                            Quantity = 1
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = products[1].Id,
                            Quantity = 1
                        }
                    ]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = users[1].Id,
                    AddressId = addresses[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    Items =
                    [
                        new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = products[2].Id,
                            Quantity = 2
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = products[3].Id,
                            Quantity = 1
                        }
                    ]
                }
            ];
        }
    }
}
