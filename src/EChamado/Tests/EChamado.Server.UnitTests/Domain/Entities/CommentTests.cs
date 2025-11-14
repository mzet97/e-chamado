using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class CommentTests : UnitTestBase
{
    [Fact]
    public void Create_WithValidData_ShouldCreateComment()
    {
        // Arrange
        var text = "Comentário de teste";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail);

        // Assert
        comment.Should().NotBeNull();
        comment.Text.Should().Be(text);
        comment.OrderId.Should().Be(orderId);
        comment.UserId.Should().Be(userId);
        comment.UserEmail.Should().Be(userEmail);
        comment.Id.Should().NotBe(Guid.Empty);
        comment.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        comment.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidComment()
    {
        // Arrange
        var text = ""; // Texto inválido
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail);

        // Assert
        comment.Should().NotBeNull();
        comment.IsValid().Should().BeFalse();
        comment.GetErrors().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Comentário simples")]
    [InlineData("Comentário com números 123")]
    [InlineData("Comentário\ncom\nquebras\nde\nlinha")]
    [InlineData("Comentário com símbolos @#$%^&*()")]
    public void Create_WithDifferentValidTexts_ShouldCreateValidComment(string text)
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail);

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
        var text = "Comentário de teste";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var comment = Comment.Create(text, orderId, userId, email);

        // Assert
        comment.Should().NotBeNull();
        comment.UserEmail.Should().Be(email);
        comment.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldSetDefaultProperties()
    {
        // Arrange
        var text = "Comentário de teste";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail);

        // Assert
        comment.Id.Should().NotBe(Guid.Empty);
        comment.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        comment.UpdatedAt.Should().BeNull();
        comment.DeletedAt.Should().BeNull();
        comment.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Create_ShouldTriggerValidation()
    {
        // Arrange & Act
        var validComment = Comment.Create("Texto válido", Guid.NewGuid(), Guid.NewGuid(), "user@test.com");
        var invalidComment = Comment.Create("", Guid.NewGuid(), Guid.NewGuid(), "user@test.com");

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
        var comment = Comment.Create(maxLengthText, orderId, userId, userEmail);

        // Assert
        comment.Should().NotBeNull();
        comment.Text.Should().Be(maxLengthText);
        comment.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyGuids_ShouldCreateInvalidComment()
    {
        // Arrange
        var text = "Comentário válido";
        var orderId = Guid.Empty; // ID inválido
        var userId = Guid.Empty; // ID inválido
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(text, orderId, userId, userEmail);

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
        var text = "Comentário válido";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var comment = Comment.Create(text, orderId, userId, invalidEmail);

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
        // Verificar que as propriedades são apenas leitura (não há setters públicos)
        var textProperty = typeof(Comment).GetProperty(nameof(Comment.Text));
        var orderIdProperty = typeof(Comment).GetProperty(nameof(Comment.OrderId));
        var userIdProperty = typeof(Comment).GetProperty(nameof(Comment.UserId));
        var userEmailProperty = typeof(Comment).GetProperty(nameof(Comment.UserEmail));

        textProperty!.SetMethod.Should().NotBeNull(); // Setter privado existe
        textProperty.SetMethod!.IsPublic.Should().BeFalse(); // Mas não é público

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
        var textWithSpecialChars = "Texto com acentos: ção, ã, é, í, ó, ú\nCom quebra de linha\tCom tab";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment = Comment.Create(textWithSpecialChars, orderId, userId, userEmail);

        // Assert
        comment.Should().NotBeNull();
        comment.Text.Should().Be(textWithSpecialChars);
        comment.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_MultipleComments_ShouldHaveUniqueIds()
    {
        // Arrange
        var text = "Comentário de teste";
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userEmail = "user@test.com";

        // Act
        var comment1 = Comment.Create(text, orderId, userId, userEmail);
        var comment2 = Comment.Create(text, orderId, userId, userEmail);

        // Assert
        comment1.Id.Should().NotBe(comment2.Id);
        comment1.CreatedAt.Should().BeCloseTo(comment2.CreatedAt, TimeSpan.FromSeconds(1));
    }
}