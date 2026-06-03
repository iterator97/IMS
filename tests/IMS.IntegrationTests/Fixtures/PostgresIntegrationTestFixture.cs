using IMS.Domain;
using IMS.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace IMS.IntegrationTests.Fixtures;

public sealed class PostgresIntegrationTestFixture : IAsyncLifetime
{
    private readonly DbContextOptions<AppDbContext> _options;

    public PostgresIntegrationTestFixture()
    {
        var connectionString = GetConnectionString();

        _options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;
    }

    public async Task InitializeAsync()
    {
        await using var context = CreateContext();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public AppDbContext CreateContext()
    {
        return new AppDbContext(_options);
    }

    private static string GetConnectionString()
    {
        var envPath = Path.Combine(AppContext.BaseDirectory, ".env");

        return File.ReadAllText(envPath).Trim();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateContext();

        context.Products.RemoveRange(context.Products);
        await context.SaveChangesAsync();
    }

    public async Task ResetAndSeedAsync()
    {
        await ResetDatabaseAsync();

        await using var context = CreateContext();
        context.Products.AddRange(TestProducts());
        await context.SaveChangesAsync();
    }

    private static Product[] TestProducts()
    {
        return
        [
            new Product
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Laptop Lenovo ThinkPad",
                Description = "Laptop biznesowy Intel i5.",
                Price = 4299.99m,
                Stock = 10
            },
            new Product
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Monitor Dell",
                Description = "Monitor QHD IPS.",
                Price = 1299.00m,
                Stock = 15
            },
            new Product
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Klawiatura Logitech",
                Description = "Klawiatura mechaniczna.",
                Price = 349.99m,
                Stock = 30
            },
            new Product
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Mysz Logitech",
                Description = "Mysz do pracy.",
                Price = 429.99m,
                Stock = 25
            },
            new Product
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Sluchawki Sony",
                Description = "Sluchawki bezprzewodowe.",
                Price = 1499.99m,
                Stock = 8
            }
        ];
    }
}
