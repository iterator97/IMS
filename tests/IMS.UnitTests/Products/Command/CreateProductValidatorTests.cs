using IMS.Application.Products.Command;
using IMS.Application.Products.Dto;

namespace IMS.UnitTests.Products.Command;

public sealed class CreateProductValidatorTests
{
    private readonly CreateProduct.Validator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenNameIsMissing_ShouldReturnError(string? name)
    {
        var command = CreateCommand(name: name);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ProductDto.Name");
    }

    [Fact]
    public void Validate_WhenNameIsLongerThan50Characters_ShouldReturnError()
    {
        var command = CreateCommand(name: new string('a', 51));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ProductDto.Name");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WhenDescriptionIsMissing_ShouldReturnError(string? description)
    {
        var command = CreateCommand(description: description);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ProductDto.Description");
    }

    [Fact]
    public void Validate_WhenDescriptionIsLongerThan50Characters_ShouldReturnError()
    {
        var command = CreateCommand(description: new string('a', 51));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ProductDto.Description");
    }

    [Fact]
    public void Validate_WhenPriceIsNull_ShouldReturnError()
    {
        var command = CreateCommand(price: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ProductDto.Price");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenPriceIsNotGreaterThanZero_ShouldReturnError(decimal price)
    {
        var command = CreateCommand(price: price);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ProductDto.Price");
    }

    [Fact]
    public void Validate_WhenStockIsNull_ShouldReturnError()
    {
        var command = CreateCommand(stock: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ProductDto.Stock");
    }

    [Fact]
    public void Validate_WhenStockIsNegative_ShouldReturnError()
    {
        var command = CreateCommand(stock: -1);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "ProductDto.Stock");
    }

    private static CreateProduct.Command CreateCommand(
        string? name = "Product name",
        string? description = "Product description",
        decimal? price = 10,
        int? stock = 5)
    {
        return new CreateProduct.Command
        {
            ProductDto = new CreateProductDto
            {
                Name = name,
                Description = description,
                Price = price,
                Stock = stock
            }
        };
    }
}
