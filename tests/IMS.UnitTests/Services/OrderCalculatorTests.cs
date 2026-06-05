using IMS.Application.Services;
using IMS.Domain;

namespace IMS.UnitTests.Services;

public sealed class OrderCalculatorTests
{
    private readonly OrderCalculator _orderCalculator;

    private readonly List<User> _users;
    private readonly List<Address> _addresses;
    private readonly List<Product> _products;
    private readonly List<Discount> _discounts;

    public OrderCalculatorTests()
    {
        _orderCalculator = new OrderCalculator();
        _users = GetUsers();
        _addresses = GetAddresses(_users);
        _products = GetProducts();
        _discounts = GetDiscounts();
    }

    [Fact]
    public void Calculate_ForGivenEuropeInputs_ShouldCalculateCorrect()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _users[0].Id,
            AddressId = _addresses[0].Id,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[0].Id, Quantity = 1 },
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[1].Id, Quantity = 1 }
            ]
        };

        // Act
        var result = _orderCalculator.Calculate(
            order,
            _addresses[0].Region,
            _products.ToDictionary(product => product.Id));

        // 1 * 4299.99m
        // 1 * 1299.00m
        // (4299.99m + 1299.00m) * (1 + 0.15) 
        // 6438.8385 rounded to 2 decimal places = 6438.84

        // Assert
        Assert.Equal(0.15m, result.LocationCharge);
        Assert.Equal(6438.84m, result.TotalAmount);
    }

    [Fact]
    public void Calculate_ForGivenAsiaInputs_ShouldCalculateCorrect()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _users[1].Id,
            AddressId = _addresses[1].Id,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[1].Id, Quantity = 145 },
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[2].Id, Quantity = 198 }
            ]
        };

        // Act
        var result = _orderCalculator.Calculate(
            order,
            _addresses[1].Region,
            _products.ToDictionary(product => product.Id));

        // 145 * 1299.00 * 0.7 = 131,848.5
        // 198 * 349.99 * 0.7 = 48508.614
        // 131,848.5 + 48508.614 * (1 + 0.05) = 189,374.9697
        // 189,374.9697 rounded to 2 decimal places = 189,374.97

        // Assert
        Assert.Equal(0.05m, result.LocationCharge);
        Assert.Equal(189374.97m, result.TotalAmount);
    }

    [Fact]
    public void Calculate_ForGivenEuropeInputs_ShouldCalculateCorrectWithDiscountOnAllProducts()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _users[1].Id,
            AddressId = _addresses[0].Id,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[1].Id, Quantity = 10 },
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[2].Id, Quantity = 6 }
            ]
        };

        // Act
        var result = _orderCalculator.Calculate(
            order,
            _addresses[0].Region,
            _products.ToDictionary(product => product.Id),
            _discounts[0]);

        // 10 * 1299.00 * 0.6 = 7,794.00.
        // 6 * 349.99 * 0.6 = 1259.964
        // (7,794.00 + 1259.964) * (1 + 0.05) = 9,053.964 * 1.15 = 10,412.0586
        // 10,412.0586 rounded to 2 decimal places = 10,412.06

        // Assert
        Assert.Equal(0.15m, result.LocationCharge);
        Assert.Equal(10412.06m, result.TotalAmount);
    }

    [Fact]
    public void Calculate_ForGivenUSInputs_ShouldCalculateCorrectWithDiscountOnHighestProduct()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _users[1].Id,
            AddressId = _addresses[2].Id,
            CreatedAt = DateTime.UtcNow,
            Items =
            [
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[1].Id, Quantity = 10 },
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[2].Id, Quantity = 6 }
            ]
        };

        // Act
        var result = _orderCalculator.Calculate(
            order,
            _addresses[2].Region,
            _products.ToDictionary(product => product.Id),
            _discounts[1]);

        // 10 * 1299.00 * 0.6 = 7,794.00.
        // 6 * 349.99 * 0.9 = 1,889.946
        // (7,794.00 + 1,889.946) = 9,683.946
        // 9,683.946 rounded to 2 decimal places = 9,683.95

        // Assert
        Assert.Equal(0, result.LocationCharge);
        Assert.Equal(9683.95m, result.TotalAmount);
    }

    [Fact]
    public void Calculate_ForGivenUSInputs_ShouldCalculateCorrectWithoutDiscount()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = _users[1].Id,
            AddressId = _addresses[2].Id,
            CreatedAt = DateTime.UtcNow.AddMonths(-1),
            Items =
            [
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[1].Id, Quantity = 10 },
                new OrderItem { Id = Guid.NewGuid(), ProductId = _products[2].Id, Quantity = 6 }
            ]
        };

        // Act
        var result = _orderCalculator.Calculate(
            order,
            _addresses[2].Region,
            _products.ToDictionary(product => product.Id),
            null);

        // 10 * 1299.00 * 0.8 = 10,392.00
        // 6 * 349.99 * 0.9 = 1,889.946
        // (10,392.00 + 1,889.946) = 12,281.946
        // 12,281.946 rounded to 2 decimal places = 12,281.95

        // Assert
        Assert.Equal(0, result.LocationCharge);
        Assert.Equal(12281.95m, result.TotalAmount);
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

    private static List<Product> GetProducts()
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

    private static List<Discount> GetDiscounts()
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
