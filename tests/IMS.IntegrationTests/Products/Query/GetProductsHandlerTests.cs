using IMS.Application.Helpers;
using IMS.Application.Products.Query;
using IMS.IntegrationTests.Fixtures;

namespace IMS.IntegrationTests.Products.Query;

[Collection(IntegrationTestCollection.Name)]
public sealed class GetProductsHandlerTests(PostgresIntegrationTestFixture fixture)
{
    [Fact]
    public async Task Handle_WhenProductsExist_ShouldReturnPagedData()
    {
        // Arrange
        await fixture.ResetAndSeedAsync();

        await using var context = fixture.CreateContext();

        var handler = new GetProducts.Handler(context);

        // Act
        var result = await handler.Handle(
            new GetProducts.Query { QueryParams = new QueryParams() },
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(5, result.Value.TotalCount);
        Assert.Equal(5, result.Value.Items.Count);
    }
}
