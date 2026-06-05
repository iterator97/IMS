using IMS.Infrastructure;
using IMS.IntegrationTests.Mock;
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
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public AppDbContext CreateContext()
    {
        return new AppDbContext(_options);
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateContext();

        context.OrderItems.RemoveRange(context.OrderItems);
        context.Orders.RemoveRange(context.Orders);
        context.Addresses.RemoveRange(context.Addresses);
        context.Users.RemoveRange(context.Users);
        context.Discounts.RemoveRange(context.Discounts);
        context.Products.RemoveRange(context.Products);

        await context.SaveChangesAsync();
    }

    public async Task ResetAndSeedAsync()
    {
        await ResetDatabaseAsync();

        await using var context = CreateContext();

        context.Products.AddRange(MockData.TestProducts());
        context.Users.AddRange(MockData.TestUsers());
        context.Addresses.AddRange(MockData.TestAddresses());
        context.Discounts.AddRange(MockData.TestDiscounts());

        await context.SaveChangesAsync();
    }

    private static string GetConnectionString()
    {
        var envPath = Path.Combine(AppContext.BaseDirectory, ".env");

        return File.ReadAllText(envPath).Trim();
    }
}
