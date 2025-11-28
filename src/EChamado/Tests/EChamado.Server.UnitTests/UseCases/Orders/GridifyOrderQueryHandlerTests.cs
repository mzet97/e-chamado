using EChamado.Server.Application.UseCases.Orders.Queries;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.UseCases.Orders;

public class GridifyOrderQueryHandlerTests
{
    [Fact]
    public void GridifyOrderQuery_ShouldImplement_IGridifyQuery()
    {
        // Arrange & Act
        var query = new GridifyOrderQuery
        {
            Page = 1,
            PageSize = 10,
            Filter = "Title == \"Test\"",
            OrderBy = "CreatedAt desc"
        };

        // Assert
        query.Should().NotBeNull();
        query.Page.Should().Be(1);
        query.PageSize.Should().Be(10);
        query.Filter.Should().Be("Title == \"Test\"");
        query.OrderBy.Should().Be("CreatedAt desc");
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(5, 50)]
    public void GridifyOrderQuery_WithDifferentPaginationValues_ShouldAcceptValues(int page, int pageSize)
    {
        // Arrange & Act
        var query = new GridifyOrderQuery
        {
            Page = page,
            PageSize = pageSize
        };

        // Assert
        query.Page.Should().Be(page);
        query.PageSize.Should().Be(pageSize);
    }

    [Fact]
    public void GridifyOrderQuery_WithFilterValues_ShouldAcceptValues()
    {
        // Arrange
        var testFilter = "StatusId == @0";
        var testOrderBy = "CreatedAt desc, Title asc";

        // Act
        var query = new GridifyOrderQuery
        {
            Filter = testFilter,
            OrderBy = testOrderBy,
            Page = 1,
            PageSize = 10
        };

        // Assert
        query.Filter.Should().Be(testFilter);
        query.OrderBy.Should().Be(testOrderBy);
    }

    [Fact]
    public void GridifyOrderQuery_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var query = new GridifyOrderQuery();

        // Assert
        query.Page.Should().Be(1);
        query.PageSize.Should().Be(10);
        query.Filter.Should().BeNull();
        query.OrderBy.Should().BeNull();
    }
}
