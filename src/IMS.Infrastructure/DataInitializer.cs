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
            if (context.Products.Any()) return;

            var products = new List<Product>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Laptop Lenovo ThinkPad",
                    Description = "Laptop biznesowy Intel i5, 16 GB RAM, SSD 512 GB.",
                    Price = 4299.99m,
                    Stock = 10
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Monitor Dell 27\"",
                    Description = "Monitor QHD IPS z regulacją wysokości.",
                    Price = 1299.00m,
                    Stock = 15
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Klawiatura Logitech",
                    Description = "Mechaniczna klawiatura RGB z przełącznikami.",
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
                    Name = "Słuchawki Sony WH-1000XM5",
                    Description = "Bezprzewodowe słuchawki z redukcją szumów.",
                    Price = 1499.99m,
                    Stock = 8
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Dysk SSD Samsung 1TB",
                    Description = "Szybki dysk SSD NVMe o pojemności 1 TB.",
                    Price = 399.99m,
                    Stock = 40
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Smartfon Samsung Galaxy",
                    Description = "Smartfon AMOLED, 128 GB pamięci, aparat 50 MP.",
                    Price = 2499.00m,
                    Stock = 12
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "Tablet Apple iPad",
                    Description = "Tablet Retina 64 GB z obsługą Apple Pencil.",
                    Price = 1999.99m,
                    Stock = 6
                }
            };

            context.Products.AddRange(products);

            await context.SaveChangesAsync();
        }
    }
}
