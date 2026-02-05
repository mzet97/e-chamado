using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Orders.Queries;
using EChamado.Server.Application.UseCases.Orders.ViewModels;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace EChamado.Server.UnitTests.Common;

public class GridifyQueryValidatorTests
{
    private readonly GridifyQueryValidator<GridifyOrderQuery, OrderViewModel> _validator;

    public GridifyQueryValidatorTests()
    {
        _validator = new GridifyQueryValidator<GridifyOrderQuery, OrderViewModel>();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(10000)]
    public void Page_WithValidValues_ShouldNotHaveValidationError(int page)
    {
        // Arrange
        var query = new GridifyOrderQuery { Page = page, PageSize = 10 };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10001)]
    public void Page_WithInvalidValues_ShouldHaveValidationError(int page)
    {
        // Arrange
        var query = new GridifyOrderQuery { Page = page, PageSize = 10 };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.Page);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void PageSize_WithValidValues_ShouldNotHaveValidationError(int pageSize)
    {
        // Arrange
        var query = new GridifyOrderQuery { Page = 1, PageSize = pageSize };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.PageSize);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(1000)]
    public void PageSize_WithInvalidValues_ShouldHaveValidationError(int pageSize)
    {
        // Arrange
        var query = new GridifyOrderQuery { Page = 1, PageSize = pageSize };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.PageSize);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Name == \"Test\"")]
    [InlineData("CreatedAt >= 2024-01-01")]
    public void Filter_WithValidValues_ShouldNotHaveValidationError(string? filter)
    {
        // Arrange
        var query = new GridifyOrderQuery { Page = 1, PageSize = 10, Filter = filter };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.Filter);
    }

    [Fact]
    public void Filter_WithExcessiveLength_ShouldHaveValidationError()
    {
        // Arrange
        var longFilter = new string('a', 501);
        var query = new GridifyOrderQuery { Page = 1, PageSize = 10, Filter = longFilter };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.Filter);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("Name")]
    [InlineData("CreatedAt desc")]
    [InlineData("Name asc, CreatedAt desc")]
    public void OrderBy_WithValidValues_ShouldNotHaveValidationError(string? orderBy)
    {
        // Arrange
        var query = new GridifyOrderQuery { Page = 1, PageSize = 10, OrderBy = orderBy };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(q => q.OrderBy);
    }

    [Fact]
    public void OrderBy_WithExcessiveLength_ShouldHaveValidationError()
    {
        // Arrange
        var longOrderBy = new string('a', 201);
        var query = new GridifyOrderQuery { Page = 1, PageSize = 10, OrderBy = longOrderBy };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(q => q.OrderBy);
    }

    [Fact]
    public void ValidQuery_ShouldNotHaveAnyValidationErrors()
    {
        // Arrange
        var query = new GridifyOrderQuery
        {
            Page = 1,
            PageSize = 10,
            Filter = "Title @= \"Test\"",
            OrderBy = "CreatedAt desc"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
