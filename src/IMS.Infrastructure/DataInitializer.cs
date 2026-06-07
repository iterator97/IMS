using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMS.Domain.Discounts;
using IMS.Domain.Orders;
using IMS.Domain.Products;
using IMS.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace IMS.Infrastructure
{
    public class DbInitializer
    {
        public static async Task SeedData(AppDbContext context)
        {
            var products = GetProducts();
            var users = GetUsers();
            var addresses = GetAddresses(users);
            var discounts = GetDiscounts();

            if (!await context.Products.AnyAsync())
            {
                context.Products.AddRange(products);
            }

            if (!await context.Users.AnyAsync())
            {
                context.Users.AddRange(users);
            }

            if (!await context.Discounts.AnyAsync())
            {
                context.Discounts.AddRange(discounts);
            }

            if (!await context.Addresses.AnyAsync())
            {
                context.Addresses.AddRange(addresses);
            }

            if (!await context.Orders.AnyAsync())
            {
                context.Orders.AddRange(GetOrders(users, addresses, products));
            }

            await context.SaveChangesAsync();
        }

        private static List<Product> GetProducts()
        {
            return
            [
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
                new()
                {
                    Id = Guid.Parse("ae8b6e23-3424-4cd9-8dd6-b22ca4033c75"),
                    Name = "Mysz Logitech MX Master 3S",
                    Description = "Ergonomiczna mysz do pracy biurowej.",
                    Price = 429.99m,
                    Stock = 25
                },
                new()
                {
                    Id = Guid.Parse("432e66c7-9ef9-405a-a100-25e0a0fe0a39"),
                    Name = "Sluchawki Sony WH-1000XM5",
                    Description = "Sluchawki z redukcja szumow.",
                    Price = 1499.99m,
                    Stock = 8
                },
                new()
                {
                    Id = Guid.Parse("5159bdb1-f787-4fba-9c5c-bb60b789250b"),
                    Name = "Dysk SSD Samsung 1TB",
                    Description = "Szybki dysk SSD NVMe o pojemnosci 1 TB.",
                    Price = 399.99m,
                    Stock = 40
                },
                new()
                {
                    Id = Guid.Parse("46c9f4eb-eece-4a8f-b834-06e7a8e1ee84"),
                    Name = "Smartfon Samsung Galaxy",
                    Description = "Smartfon AMOLED, 128 GB pamieci.",
                    Price = 2499.00m,
                    Stock = 12
                },
                new()
                {
                    Id = Guid.Parse("53cd7118-3120-4bc9-a226-9bf098d6ae4a"),
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
                    Id = Guid.Parse("b22d4436-5576-4000-bcaf-053cbbe91941"),
                    Name = "Anna",
                    Surname = "Kowalska",
                    Email = "anna.kowalska@example.com"
                },
                new()
                {
                    Id = Guid.Parse("429891f4-8501-4851-b109-7ef72d86c797"),
                    Name = "Jan",
                    Surname = "Nowak",
                    Email = "jan.nowak@example.com"
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

        private static List<Address> GetAddresses(IReadOnlyList<User> users)
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
                    Region = "Europe",
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
                    Region = "Asia",
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
                    LocationCharge = 0.05m,
                    TotalAmount = 0,
                    Items =
                    [
                        new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = products[0].Id,
                            Quantity = 1,
                            Discount = 0
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = products[1].Id,
                            Quantity = 1,
                            Discount = 0
                        }
                    ]
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    UserId = users[1].Id,
                    AddressId = addresses[1].Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    LocationCharge = 0.05m,
                    TotalAmount = 0,
                    Items =
                    [
                        new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = products[2].Id,
                            Quantity = 2,
                            Discount = 0
                        },
                        new()
                        {
                            Id = Guid.NewGuid(),
                            ProductId = products[3].Id,
                            Quantity = 1,
                            Discount = 0
                        }
                    ]
                }
            ];
        }
    }
}
