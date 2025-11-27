using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Services;
using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Validations;

public class CommentValidationTests : UnitTestBase
{
    private readonly CommentValidation _validator;

    public CommentValidationTests()
    {
        _validator = new CommentValidation();
    }

    [Fact]
    public void Validate_ValidComment_ShouldPass()
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Validate_EmptyOrWhitespaceText_ShouldFail(string text)
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithText(text)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Text");
        result.Errors.First().ErrorMessage.Should().Contain("obrigatório");
    }

    [Fact]
    public void Validate_TextTooLong_ShouldFail()
    {
        // Arrange
        var longText = new string('A', 2001); // Excede o limite de 2000 caracteres
        var comment = CommentTestBuilder.Create()
            .WithText(longText)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Text");
        result.Errors.First(e => e.PropertyName == "Text").ErrorMessage.Should().Contain("2000 caracteres");
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@invalid.com")]
    [InlineData("invalid@")]
    [InlineData("invalid.com")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_InvalidEmail_ShouldFail(string email)
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithUserEmail(email)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserEmail");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@domain.com.br")]
    [InlineData("user123@test-domain.org")]
    [InlineData("admin@localhost")]
    public void Validate_ValidEmail_ShouldPass(string email)
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithUserEmail(email)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_EmptyOrderId_ShouldFail()
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithOrderId(Guid.Empty)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldFail()
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithUserId(Guid.Empty)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Theory]
    [InlineData("Comentário simples")]
    [InlineData("Comentário com números 123 e símbolos @#$%")]
    [InlineData("Comentário\ncom\nquebras\nde\nlinha")]
    [InlineData("Comentário muito longo que testa se o sistema consegue lidar com textos extensos mas que ainda estão dentro do limite permitido de caracteres")]
    public void Validate_ValidTextVariations_ShouldPass(string text)
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithText(text)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_TextAtMaxLength_ShouldPass()
    {
        // Arrange
        var maxLengthText = new string('A', 2000); // Exatamente no limite
        var comment = CommentTestBuilder.Create()
            .WithText(maxLengthText)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_MultipleErrors_ShouldReturnAllErrors()
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithText("") // Texto vazio
            .WithUserEmail("invalid-email") // Email inválido
            .WithOrderId(Guid.Empty) // OrderId vazio
            .WithUserId(Guid.Empty) // UserId vazio
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(4);
        result.Errors.Should().Contain(e => e.PropertyName == "Text");
        result.Errors.Should().Contain(e => e.PropertyName == "UserEmail");
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Ab")]
    [InlineData("Comentário mínimo")]
    public void Validate_MinimumValidText_ShouldPass(string text)
    {
        // Arrange
        var comment = CommentTestBuilder.Create()
            .WithText(text)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_TextWithSpecialCharacters_ShouldPass()
    {
        // Arrange
        var specialText = "Comentário com acentos: ção, ã, é, í, ó, ú e símbolos: @#$%^&*()[]{}";
        var comment = CommentTestBuilder.Create()
            .WithText(specialText)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
}
