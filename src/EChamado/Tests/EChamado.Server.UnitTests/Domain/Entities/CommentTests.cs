using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Services;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class CommentTests : UnitTestBase
{
    private static readonly IDateTimeProvider _dateTimeProvider = new SystemDateTimeProvider();

    [Fact]
    public void Create_WithValidData_ShouldCreateComment()
    {
        // Arrange
        var text = "Coment�rio de teste";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail, _dateTimeProvider);

        // Assert
        comment.Should().NotBeNull();
        comment.Text.Should().Be(text);
        comment.OrderId.Should().Be(orderId);
        comment.UserId.Should().Be(userId);
        comment.UserEmail.Should().Be(userEmail);
        comment.Id.Should().NotBe(Guid.Empty);
        comment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        comment.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidComment()
    {
        // Arrange
        var text = ""; // Texto inv�lido
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail, _dateTimeProvider);

        // Assert
        comment.Should().NotBeNull();
        comment.IsValid().Should().BeFalse();
        comment.GetErrors().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Coment�rio simples")]
    [InlineData("Coment�rio com n�meros 123")]
    [InlineData("Coment�rio\ncom\nquebras\nde\nlinha")]
    [InlineData("Coment�rio com s�mbolos @#$%^&*()")]
    public void Create_WithDifferentValidTexts_ShouldCreateValidComment(string text)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail, _dateTimeProvider);

        // Assert
        comment.Should().NotBeNull();
        comment.Text.Should().Be(text);
        comment.IsValid().Should().BeTrue();
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@domain.com.br")]
    [InlineData("user123@test-domain.org")]
    [InlineData("admin@localhost")]
    public void Create_WithDifferentValidEmails_ShouldCreateValidComment(string email)
    {
        // Arrange
        var text = "Coment�rio de teste";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var comment = Comment.Create(text, orderId, userId, email, _dateTimeProvider);

        // Assert
        comment.Should().NotBeNull();
        comment.UserEmail.Should().Be(email);
        comment.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldSetDefaultProperties()
    {
        // Arrange
        var text = "Coment�rio de teste";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail, _dateTimeProvider);

        // Assert
        comment.Id.Should().NotBe(Guid.Empty);
        comment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        comment.UpdatedAt.Should().BeNull();
        comment.DeletedAt.Should().BeNull();
        comment.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldTriggerValidation()
    {
        // Arrange & Act
        var validComment = Comment.Create("Texto v�lido", Guid.NewGuid(), Guid.NewGuid(), "user@test.com", _dateTimeProvider);
        var invalidComment = Comment.Create("", Guid.NewGuid(), Guid.NewGuid(), "user@test.com", _dateTimeProvider);

        // Assert
        validComment.IsValid().Should().BeTrue();
        validComment.GetErrors().Should().BeEmpty();

        invalidComment.IsValid().Should().BeFalse();
        invalidComment.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WithMaxLengthText_ShouldWork()
    {
        // Arrange
        var maxLengthText = new string('A', 2000); // No limite
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(maxLengthText, orderId, userId, userEmail, _dateTimeProvider);

        // Assert
        comment.Should().NotBeNull();
        comment.Text.Should().Be(maxLengthText);
        comment.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyGuids_ShouldCreateInvalidComment()
    {
        // Arrange
        var text = "Coment�rio v�lido";
        var orderId = Guid.Empty; // ID inv�lido
        var userId = Guid.Empty; // ID inv�lido
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail, _dateTimeProvider);

        // Assert
        comment.Should().NotBeNull();
        comment.IsValid().Should().BeFalse();
        comment.GetErrors().Should().NotBeEmpty();
        comment.GetErrors().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid-email")]
    [InlineData("@invalid.com")]
    [InlineData("invalid@")]
    public void Create_WithInvalidEmail_ShouldCreateInvalidComment(string invalidEmail)
    {
        // Arrange
        var text = "Coment�rio v�lido";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var comment = Comment.Create(text, orderId, userId, invalidEmail, _dateTimeProvider);

        // Assert
        comment.Should().NotBeNull();
        comment.IsValid().Should().BeFalse();
        comment.GetErrors().Should().NotBeEmpty();
        comment.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Comment_ShouldHaveReadOnlyProperties()
    {
        // Arrange
        var comment = CommentTestBuilder.Create().WithValidData().Build();

        // Act & Assert
        // Verificar que as propriedades s�o apenas leitura (n�o h� setters p�blicos)
        var textProperty = typeof(Comment).GetProperty(nameof(Comment.Text));
        var orderIdProperty = typeof(Comment).GetProperty(nameof(Comment.OrderId));
        var userIdProperty = typeof(Comment).GetProperty(nameof(Comment.UserId));
        var userEmailProperty = typeof(Comment).GetProperty(nameof(Comment.UserEmail));

        textProperty!.SetMethod.Should().NotBeNull(); // Setter privado existe
        textProperty.SetMethod!.IsPublic.Should().BeFalse(); // Mas n�o � p�blico

        orderIdProperty!.SetMethod.Should().NotBeNull();
        orderIdProperty.SetMethod!.IsPublic.Should().BeFalse();

        userIdProperty!.SetMethod.Should().NotBeNull();
        userIdProperty.SetMethod!.IsPublic.Should().BeFalse();

        userEmailProperty!.SetMethod.Should().NotBeNull();
        userEmailProperty.SetMethod!.IsPublic.Should().BeFalse();
    }

    [Fact]
    public void Create_WithSpecialCharacters_ShouldPreserveText()
    {
        // Arrange
        var textWithSpecialChars = "Texto com acentos: ��o, �, �, �, �, �\nCom quebra de linha\tCom tab";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(textWithSpecialChars, orderId, userId, userEmail, _dateTimeProvider);

        // Assert
        comment.Should().NotBeNull();
        comment.Text.Should().Be(textWithSpecialChars);
        comment.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_MultipleComments_ShouldHaveUniqueIds()
    {
        // Arrange
        var text = "Coment�rio de teste";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment1 = Comment.Create(text, orderId, userId, userEmail, _dateTimeProvider);
        var comment2 = Comment.Create(text, orderId, userId, userEmail, _dateTimeProvider);

        // Assert
        comment1.Id.Should().NotBe(comment2.Id);
        comment1.CreatedAt.Should().BeCloseTo(comment2.CreatedAt, TimeSpan.FromSeconds(1));
    }
}