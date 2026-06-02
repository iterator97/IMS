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
                Description = "Biznesowy laptop z procesorem Intel Core i5, 16 GB RAM i dyskiem SSD 512 GB.",
                Price = 4299.99m,
                Stock = 10
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Monitor Dell 27\"",
                Description = "Monitor 27 cali z rozdzielczością QHD, matrycą IPS i regulacją wysokości.",
                Price = 1299.00m,
                Stock = 15
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Klawiatura mechaniczna Logitech",
                Description = "Klawiatura mechaniczna z podświetleniem RGB i przełącznikami tactile.",
                Price = 349.99m,
                Stock = 30
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Mysz bezprzewodowa Logitech MX Master 3S",
                Description = "Ergonomiczna mysz bezprzewodowa do pracy biurowej i programowania.",
                Price = 429.99m,
                Stock = 25
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Słuchawki Sony WH-1000XM5",
                Description = "Bezprzewodowe słuchawki z aktywną redukcją szumów.",
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
                Description = "Smartfon z ekranem AMOLED, 128 GB pamięci i aparatem 50 MP.",
                Price = 2499.00m,
                Stock = 12
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Tablet Apple iPad",
                Description = "Tablet z ekranem Retina, 64 GB pamięci i obsługą Apple Pencil.",
                Price = 1999.99m,
                Stock = 6
            }
        };

            context.Products.AddRange(products);

            await context.SaveChangesAsync();
        }
    }
}
