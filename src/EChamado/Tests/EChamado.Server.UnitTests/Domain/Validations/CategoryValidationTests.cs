using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Services;
using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Validations;

public class CategoryValidationTests : UnitTestBase
{
    private readonly CategoryValidation _validator;

    public CategoryValidationTests()
    {
        _validator = new CategoryValidation();
    }

    [Fact]
    public void Validate_ValidCategory_ShouldPass()
    {
        // Arrange
        var category = CategoryTestBuilder.Create()
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("", "Descrição válida")]
    [InlineData("   ", "Descrição válida")]
    [InlineData("\t", "Descrição válida")]
    public void Validate_EmptyOrWhitespaceName_ShouldFail(string name, string description)
    {
        // Arrange
        var category = CategoryTestBuilder.Create()
            .WithName(name)
            .WithDescription(description)
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.First().ErrorMessage.Should().Contain("obrigatório");
    }

    [Fact]
    public void Validate_NameTooLong_ShouldFail()
    {
        // Arrange
        var longName = new string('A', 101); // Excede o limite de 100 caracteres
        var category = CategoryTestBuilder.Create()
            .WithName(longName)
            .WithDescription("Descrição válida")
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.First(e => e.PropertyName == "Name").ErrorMessage.Should().Contain("100 caracteres");
    }

    [Fact]
    public void Validate_NameTooShort_ShouldFail()
    {
        // Arrange
        var shortName = "A"; // Menor que 2 caracteres
        var category = CategoryTestBuilder.Create()
            .WithName(shortName)
            .WithDescription("Descrição válida")
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public void Validate_EmptyOrWhitespaceDescription_ShouldFail(string description)
    {
        // Arrange
        var category = CategoryTestBuilder.Create()
            .WithName("Categoria Válida")
            .WithDescription(description)
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Validate_DescriptionTooLong_ShouldFail()
    {
        // Arrange
        var longDescription = new string('A', 501); // Excede o limite de 500 caracteres
        var category = CategoryTestBuilder.Create()
            .WithName("Categoria Válida")
            .WithDescription(longDescription)
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
        result.Errors.First(e => e.PropertyName == "Description").ErrorMessage.Should().Contain("500 caracteres");
    }

    [Theory]
    [InlineData("Categoria Normal", "Descrição normal")]
    [InlineData("Ca", "D")] // Valores mínimos
    [InlineData("Cat-egoria_123", "Descrição com números 123 e símbolos!")]
    [InlineData("CATEGORIA MAIÚSCULA", "DESCRIÇÃO MAIÚSCULA")]
    [InlineData("categoria minúscula", "descrição minúscula")]
    public void Validate_ValidNameAndDescriptionVariations_ShouldPass(string name, string description)
    {
        // Arrange
        var category = CategoryTestBuilder.Create()
            .WithName(name)
            .WithDescription(description)
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("Nome com @#$%", "Descrição normal")]
    [InlineData("Nome<script>", "Descrição normal")]
    [InlineData("Nome normal", "Descrição com <script>alert('xss')</script>")]
    public void Validate_NameOrDescriptionWithSpecialCharacters_ShouldHandleCorrectly(string name, string description)
    {
        // Arrange
        var category = CategoryTestBuilder.Create()
            .WithName(name)
            .WithDescription(description)
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        // Dependendo das regras de negócio, isso pode passar ou falhar
        // Por enquanto, vamos assumir que caracteres especiais são permitidos
        if (name.Length >= 2 && name.Length <= 100 && 
            description.Length >= 1 && description.Length <= 500)
        {
            result.IsValid.Should().BeTrue();
        }
    }

    [Fact]
    public void Validate_BoundaryValues_ShouldWork()
    {
        // Arrange - Nome com exatamente 100 caracteres
        var maxLengthName = new string('A', 100);
        var maxLengthDescription = new string('B', 500);
        
        var category = CategoryTestBuilder.Create()
            .WithName(maxLengthName)
            .WithDescription(maxLengthDescription)
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var category = CategoryTestBuilder.Create()
            .WithName("") // Nome vazio
            .WithDescription("") // Descrição vazia
            .Build();

        // Act
        var result = _validator.Validate(category);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "Description");
    }
}
