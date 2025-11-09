using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Validations;

public class CommentValidationTests
{
    [Fact]
    public void Validate_ValidComment_ShouldPass()
    {
        // Arrange
        var comment = Comment.Create(
            "Comentário válido",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user@example.com"
        );
        var validator = new CommentValidation();

        // Act
        var result = validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Validate_EmptyText_ShouldFail(string text)
    {
        // Arrange
        var comment = Comment.Create(
            text ?? string.Empty,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user@example.com"
        );
        var validator = new CommentValidation();

        // Act
        var result = validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle(e => e.PropertyName == "Text");
    }

    [Fact]
    public void Validate_TextTooLong_ShouldFail()
    {
        // Arrange
        var longText = new string('A', 2001); // Excede o limite de 2000 caracteres
        var comment = Comment.Create(
            longText,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "user@example.com"
        );
        var validator = new CommentValidation();

        // Act
        var result = validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Text");
    }

    [Fact]
    public void Validate_InvalidEmail_ShouldFail()
    {
        // Arrange
        var comment = Comment.Create(
            "Comentário",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "invalid-email"
        );
        var validator = new CommentValidation();

        // Act
        var result = validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserEmail");
    }

    [Fact]
    public void Validate_EmptyOrderId_ShouldFail()
    {
        // Arrange
        var comment = Comment.Create(
            "Comentário",
            Guid.Empty,
            Guid.NewGuid(),
            "user@example.com"
        );
        var validator = new CommentValidation();

        // Act
        var result = validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "OrderId");
    }

    [Fact]
    public void Validate_EmptyUserId_ShouldFail()
    {
        // Arrange
        var comment = Comment.Create(
            "Comentário",
            Guid.NewGuid(),
            Guid.Empty,
            "user@example.com"
        );
        var validator = new CommentValidation();

        // Act
        var result = validator.Validate(comment);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "UserId");
    }
}
