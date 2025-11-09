using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Validations;

public class CategoryValidationTests
{
    [Fact]
    public void Validate_ValidCategory_ShouldPass()
    {
        // Arrange
        var category = Category.Create("Categoria Teste", "Descrição válida");
        var validator = new CategoryValidation();

        // Act
        var result = validator.Validate(category);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Descrição")]
    [InlineData(null, "Descrição")]
    public void Validate_EmptyName_ShouldFail(string name, string description)
    {
        // Arrange
        var category = Category.Create(name ?? string.Empty, description);
        var validator = new CategoryValidation();

        // Act
        var result = validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldFail()
    {
        // Arrange
        var longName = new string('A', 101); // Excede o limite de 100 caracteres
        var category = Category.Create(longName, "Descrição");
        var validator = new CategoryValidation();

        // Act
        var result = validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_DescriptionTooLong_ShouldFail()
    {
        // Arrange
        var longDescription = new string('A', 501); // Excede o limite de 500 caracteres
        var category = Category.Create("Categoria", longDescription);
        var validator = new CategoryValidation();

        // Act
        var result = validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}
