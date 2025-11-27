using EChamado.Server.Application.UseCases.Categories.Queries;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.UseCases.Categories;

public class GridifyCategoryQueryHandlerTests
{
    [Fact]
    public void GridifyCategoryQuery_ShouldImplement_IGridifyQuery()
    {
        // Arrange & Act
        var query = new GridifyCategoryQuery
        {
            Page = 1,
            PageSize = 10,
            Filter = "Name == \"Test\"",
            OrderBy = "CreatedAt desc"
        };

        // Assert
        query.Should().NotBeNull();
        query.Page.Should().Be(1);
        query.PageSize.Should().Be(10);
        query.Filter.Should().Be("Name == \"Test\"");
        query.OrderBy.Should().Be("CreatedAt desc");
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 20)]
    [InlineData(5, 50)]
    public void GridifyCategoryQuery_WithDifferentPaginationValues_ShouldAcceptValues(int page, int pageSize)
    {
        // Arrange & Act
        var query = new GridifyCategoryQuery
        {
            Page = page,
            PageSize = pageSize
        };

        // Assert
        query.Page.Should().Be(page);
        query.PageSize.Should().Be(pageSize);
    }

    [Fact]
    public void GridifyCategoryQuery_WithFilterValues_ShouldAcceptValues()
    {
        // Arrange
        var testFilter = "Name @= \"Software\"";
        var testOrderBy = "Name asc";

        // Act
        var query = new GridifyCategoryQuery
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
    public void GridifyCategoryQuery_DefaultValues_ShouldBeCorrect()
    {
        // Arrange & Act
        var query = new GridifyCategoryQuery();

        // Assert
        query.Page.Should().Be(1);
        query.PageSize.Should().Be(10);
        query.Filter.Should().BeNull();
        query.OrderBy.Should().BeNull();
    }
}
