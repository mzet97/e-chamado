using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Services;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class StatusTypeTests : UnitTestBase
{
    private static readonly IDateTimeProvider _dateTimeProvider = new SystemDateTimeProvider();

    [Fact]
    public void Create_WithValidData_ShouldCreateStatusType()
    {
        // Arrange
        var name = "Em Andamento";
        var description = "Chamado est� em andamento";

        // Act
        var statusType = StatusType.Create(name, description, _dateTimeProvider);

        // Assert
        statusType.Should().NotBeNull();
        statusType.Name.Should().Be(name);
        statusType.Description.Should().Be(description);
        statusType.Id.Should().NotBe(Guid.Empty);
        statusType.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        statusType.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidStatusType()
    {
        // Arrange
        var name = ""; // Nome inv�lido
        var description = "Descri��o v�lida";

        // Act
        var statusType = StatusType.Create(name, description, _dateTimeProvider);

        // Assert
        statusType.Should().NotBeNull();
        statusType.IsValid().Should().BeFalse();
        statusType.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateStatusType()
    {
        // Arrange
        var statusType = StatusType.Create("Status Original", "Descri��o Original", _dateTimeProvider);
        var newName = "Status Atualizado";
        var newDescription = "Descri��o Atualizada";

        // Act
        statusType.Update(newName, newDescription, _dateTimeProvider);

        // Assert
        statusType.Name.Should().Be(newName);
        statusType.Description.Should().Be(newDescription);
        statusType.UpdatedAt.Should().NotBeNull();
        statusType.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("Aberto", "Chamado rec�m criado")]
    [InlineData("Em Andamento", "Chamado sendo processado")]
    [InlineData("Fechado", "Chamado resolvido")]
    [InlineData("Cancelado", "Chamado cancelado")]
    public void Create_WithDifferentValidInputs_ShouldCreateValidStatusType(string name, string description)
    {
        // Act
        var statusType = StatusType.Create(name, description, _dateTimeProvider);

        // Assert
        statusType.Should().NotBeNull();
        statusType.Name.Should().Be(name);
        statusType.Description.Should().Be(description);
        statusType.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldSetDefaultProperties()
    {
        // Arrange
        var name = "Teste Status";
        var description = "Descri��o do status";

        // Act
        var statusType = StatusType.Create(name, description, _dateTimeProvider);

        // Assert
        statusType.Id.Should().NotBe(Guid.Empty);
        statusType.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        statusType.UpdatedAt.Should().BeNull();
        statusType.DeletedAt.Should().BeNull();
        statusType.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("", "Descri��o v�lida")]
    [InlineData("   ", "Descri��o v�lida")]
    [InlineData("\t", "Descri��o v�lida")]
    public void Create_WithInvalidName_ShouldCreateInvalidStatusType(string invalidName, string description)
    {
        // Act
        var statusType = StatusType.Create(invalidName, description, _dateTimeProvider);

        // Assert
        statusType.IsValid().Should().BeFalse();
        statusType.GetErrors().Should().NotBeEmpty();
        statusType.GetErrors().Should().Contain(e => e.Contains("Name") || e.Contains("obrigat�rio"));
    }

    [Theory]
    [InlineData("Nome v�lido", "")]
    [InlineData("Nome v�lido", "   ")]
    [InlineData("Nome v�lido", "\t")]
    public void Create_WithEmptyDescription_ShouldBeValid(string name, string description)
    {
        // Act
        var statusType = StatusType.Create(name, description, _dateTimeProvider);

        // Assert
        statusType.IsValid().Should().BeTrue("Description is not required, only Name is required");
        statusType.Name.Should().Be(name);
        statusType.Description.Should().Be(description);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ShouldCreateInvalidStatusType()
    {
        // Arrange
        var name = "Nome v�lido";
        var tooLongDescription = new string('A', 501); // Excede limite de 500

        // Act
        var statusType = StatusType.Create(name, tooLongDescription, _dateTimeProvider);

        // Assert
        statusType.IsValid().Should().BeFalse();
        statusType.GetErrors().Should().NotBeEmpty();
        statusType.GetErrors().Should().Contain(e => e.Contains("Description cannot exceed 500 characters"));
    }

    [Fact]
    public void StatusType_ShouldHaveReadOnlyProperties()
    {
        // Arrange
        var statusType = StatusType.Create("Test Status", "Test Description", _dateTimeProvider);

        // Act & Assert
        var nameProperty = typeof(StatusType).GetProperty(nameof(StatusType.Name));
        var descriptionProperty = typeof(StatusType).GetProperty(nameof(StatusType.Description));

        nameProperty!.SetMethod.Should().NotBeNull();
        nameProperty.SetMethod!.IsPublic.Should().BeFalse();

        descriptionProperty!.SetMethod.Should().NotBeNull();
        descriptionProperty.SetMethod!.IsPublic.Should().BeFalse();
    }

    [Fact]
    public void Create_MultipleStatusTypes_ShouldHaveUniqueIds()
    {
        // Act
        var statusType1 = StatusType.Create("Status 1", "Description 1", _dateTimeProvider);
        var statusType2 = StatusType.Create("Status 2", "Description 2", _dateTimeProvider);

        // Assert
        statusType1.Id.Should().NotBe(statusType2.Id);
        statusType1.CreatedAt.Should().BeCloseTo(statusType2.CreatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Update_WithInvalidData_ShouldMakeStatusTypeInvalid()
    {
        // Arrange
        var statusType = StatusType.Create("Status V�lido", "Descri��o V�lida", _dateTimeProvider);

        // Act
        statusType.Update("", "Descri��o", _dateTimeProvider); // Nome vazio

        // Assert
        statusType.IsValid().Should().BeFalse();
        statusType.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var statusType = StatusType.Create("Status Original", "Descri��o Original", _dateTimeProvider);

        // Act
        statusType.Update("Status 1", "Descri��o 1", _dateTimeProvider);
        var firstUpdate = statusType.UpdatedAt;

        Thread.Sleep(100);

        statusType.Update("Status 2", "Descri��o 2", _dateTimeProvider);
        var secondUpdate = statusType.UpdatedAt;

        // Assert
        secondUpdate.Should().BeAfter(firstUpdate!.Value);
        statusType.Name.Should().Be("Status 2");
        statusType.Description.Should().Be("Descri��o 2");
    }

    [Fact]
    public void StatusType_WithLongValidData_ShouldWork()
    {
        // Arrange
        var name = new string('A', 100); // No limite assumido
        var description = new string('B', 500); // No limite assumido

        // Act
        var statusType = StatusType.Create(name, description, _dateTimeProvider);

        // Assert
        statusType.Should().NotBeNull();
        statusType.Name.Should().Be(name);
        statusType.Description.Should().Be(description);
        statusType.IsValid().Should().BeTrue();
    }

    [Fact]
    public void StatusType_ShouldInheritFromEntity()
    {
        // Arrange & Act
        var statusType = StatusType.Create("Test Status", "Test Description", _dateTimeProvider);

        // Assert
        statusType.Should().BeAssignableTo<EChamado.Shared.Shared.Entity>();
    }
}