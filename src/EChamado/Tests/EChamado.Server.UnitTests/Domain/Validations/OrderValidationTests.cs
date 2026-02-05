using EChamado.Server.Domain.Domains.Orders;
using EChamado.Shared.Services;
using EChamado.Server.Domain.Domains.Orders.Validations;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Validations;

public class OrderValidationTests : UnitTestBase
{
    private readonly OrderValidation _validator;

    public OrderValidationTests()
    {
        _validator = new OrderValidation();
    }

    [Fact]
    public void Validate_ValidOrder_ShouldPass()
    {
        // Arrange
        var order = OrderTestBuilder.Create().WithValidData().Build();

        // Act
        var result = _validator.Validate(order);

        // Assert - Se falhou, mostrar erros específicos
        if (!result.IsValid)
        {
            var errors = string.Join(", ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            Assert.Fail($"Expected valid order but got errors: {errors}");
        }

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_OrderWithEmptyTitle_ShouldFail()
    {
        // Arrange - criar order inválida sem chamar WithValidData depois
        var order = OrderTestBuilder.Create()
            .WithValidData() // Primeiro dados válidos
            .WithInvalidTitle() // Depois sobrescrever com título inválido
            .Build();

        // Act
        var result = _validator.Validate(order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        var titleErrors = result.Errors.Where(e => e.PropertyName == "Title").ToList();
        titleErrors.Should().NotBeEmpty("Should have title validation error");
    }

    [Fact]
    public void Validate_OrderWithEmptyDescription_ShouldFail()
    {
        // Arrange
        var order = OrderTestBuilder.Create()
            .WithValidData() // Primeiro dados válidos
            .WithInvalidDescription() // Depois sobrescrever com descrição inválida
            .Build();

        // Act
        var result = _validator.Validate(order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        var descErrors = result.Errors.Where(e => e.PropertyName == "Description").ToList();
        descErrors.Should().NotBeEmpty("Should have description validation error");
    }

    [Fact]
    public void Validate_OrderWithInvalidEmail_ShouldFail()
    {
        // Arrange
        var order = OrderTestBuilder.Create()
            .WithValidData() // Primeiro dados válidos
            .WithRequestingUserEmail("invalid-email") // Depois email inválido
            .Build();

        // Act
        var result = _validator.Validate(order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        var emailErrors = result.Errors.Where(e => e.PropertyName == "RequestingUserEmail").ToList();
        emailErrors.Should().NotBeEmpty("Should have email validation error");
    }

    [Fact]
    public void Validate_OrderWithEmptyGuidFields_ShouldFail()
    {
        // Arrange
        var order = OrderTestBuilder.Create()
            .WithValidData() // Primeiro dados válidos
            .WithCategoryId(Guid.Empty) // Depois ID vazio
            .Build();

        // Act
        var result = _validator.Validate(order);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
        var categoryErrors = result.Errors.Where(e => e.PropertyName == "CategoryId").ToList();
        categoryErrors.Should().NotBeEmpty("Should have category ID validation error");
    }

    [Theory]
    [InlineData("user@example.com")]
    [InlineData("test.user@domain.com.br")]
    [InlineData("user123@test-domain.org")]
    public void Validate_OrderWithValidEmails_ShouldPass(string validEmail)
    {
        // Arrange
        var order = OrderTestBuilder.Create()
            .WithRequestingUserEmail(validEmail)
            .WithResponsibleUserEmail(validEmail)
            .WithValidData()
            .Build();

        // Act
        var result = _validator.Validate(order);

        // Assert - Se falhou mostrar erros
        if (!result.IsValid)
        {
            var errors = string.Join(", ", result.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            Assert.Fail($"Expected valid emails but got errors: {errors}");
        }

        result.IsValid.Should().BeTrue();
    }
}