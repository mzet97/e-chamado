using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Services;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Shared.Services;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class OrderTypeTests : UnitTestBase
{
    private static readonly IDateTimeProvider _dateTimeProvider = new MockDateTimeProvider();

    private class MockDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
    }
    [Fact]
    public void Create_WithValidData_ShouldCreateOrderType()
    {
        // Arrange
        var name = "Suporte T�cnico";
        var description = "Solicita��es de suporte t�cnico";

        // Act
        var orderType = OrderType.Create(name, description, _dateTimeProvider);

        // Assert
        orderType.Should().NotBeNull();
        orderType.Name.Should().Be(name);
        orderType.Description.Should().Be(description);
        orderType.Id.Should().NotBe(Guid.Empty);
        orderType.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        orderType.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidOrderType()
    {
        // Arrange
        var name = ""; // Nome inv�lido
        var description = "Descri��o v�lida";

        // Act
        var orderType = OrderType.Create(name, description, _dateTimeProvider);

        // Assert
        orderType.Should().NotBeNull();
        orderType.IsValid().Should().BeFalse();
        orderType.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateOrderType()
    {
        // Arrange
        var orderType = OrderType.Create("Tipo Original", "Descri��o Original", _dateTimeProvider);
        var newName = "Tipo Atualizado";
        var newDescription = "Descri��o Atualizada";

        // Act
        orderType.Update(newName, newDescription, _dateTimeProvider);

        // Assert
        orderType.Name.Should().Be(newName);
        orderType.Description.Should().Be(newDescription);
        orderType.UpdatedAt.Should().NotBeNull();
        orderType.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("Suporte T�cnico", "Quest�es relacionadas a TI")]
    [InlineData("Recursos Humanos", "Quest�es de RH")]
    [InlineData("Financeiro", "Quest�es financeiras")]
    [InlineData("Infraestrutura", "Quest�es de infraestrutura")]
    public void Create_WithDifferentValidInputs_ShouldCreateValidOrderType(string name, string description)
    {
        // Act
        var orderType = OrderType.Create(name, description, _dateTimeProvider);

        // Assert
        orderType.Should().NotBeNull();
        orderType.Name.Should().Be(name);
        orderType.Description.Should().Be(description);
        orderType.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldSetDefaultProperties()
    {
        // Arrange
        var name = "Teste Tipo";
        var description = "Descri��o do tipo";

        // Act
        var orderType = OrderType.Create(name, description, _dateTimeProvider);

        // Assert
        orderType.Id.Should().NotBe(Guid.Empty);
        orderType.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        orderType.UpdatedAt.Should().BeNull();
        orderType.DeletedAt.Should().BeNull();
        orderType.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("", "Descri��o v�lida")]
    [InlineData("   ", "Descri��o v�lida")]
    [InlineData("\t", "Descri��o v�lida")]
    public void Create_WithInvalidName_ShouldCreateInvalidOrderType(string invalidName, string description)
    {
        // Act
        var orderType = OrderType.Create(invalidName, description, _dateTimeProvider);

        // Assert
        orderType.IsValid().Should().BeFalse();
        orderType.GetErrors().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Nome v�lido", "")]
    [InlineData("Nome v�lido", "   ")]
    [InlineData("Nome v�lido", "\t")]
    public void Create_WithInvalidDescription_ShouldCreateInvalidOrderType(string name, string invalidDescription)
    {
        // Act
        var orderType = OrderType.Create(name, invalidDescription, _dateTimeProvider);

        // Assert
        orderType.IsValid().Should().BeFalse();
        orderType.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void OrderType_ShouldHaveReadOnlyProperties()
    {
        // Arrange
        var orderType = OrderType.Create("Test Type", "Test Description", _dateTimeProvider);

        // Act & Assert
        var nameProperty = typeof(OrderType).GetProperty(nameof(OrderType.Name));
        var descriptionProperty = typeof(OrderType).GetProperty(nameof(OrderType.Description));

        nameProperty!.SetMethod.Should().NotBeNull();
        nameProperty.SetMethod!.IsPublic.Should().BeFalse();

        descriptionProperty!.SetMethod.Should().NotBeNull();
        descriptionProperty.SetMethod!.IsPublic.Should().BeFalse();
    }

    [Fact]
    public void Create_MultipleOrderTypes_ShouldHaveUniqueIds()
    {
        // Act
        var orderType1 = OrderType.Create("Tipo 1", "Descri��o 1", _dateTimeProvider);
        var orderType2 = OrderType.Create("Tipo 2", "Descri��o 2", _dateTimeProvider);

        // Assert
        orderType1.Id.Should().NotBe(orderType2.Id);
    }

    [Fact]
    public void Update_WithInvalidData_ShouldMakeOrderTypeInvalid()
    {
        // Arrange
        var orderType = OrderType.Create("Tipo V�lido", "Descri��o V�lida", _dateTimeProvider);

        // Act
        orderType.Update("", "Descri��o", _dateTimeProvider); // Nome vazio

        // Assert
        orderType.IsValid().Should().BeFalse();
        orderType.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var orderType = OrderType.Create("Tipo Original", "Descri��o Original", _dateTimeProvider);

        // Act
        orderType.Update("Tipo 1", "Descri��o 1", _dateTimeProvider);
        var firstUpdate = orderType.UpdatedAt;

        Thread.Sleep(100);

        orderType.Update("Tipo 2", "Descri��o 2", _dateTimeProvider);
        var secondUpdate = orderType.UpdatedAt;

        // Assert
        secondUpdate.Should().BeAfter(firstUpdate!.Value);
        orderType.Name.Should().Be("Tipo 2");
        orderType.Description.Should().Be("Descri��o 2");
    }

    [Fact]
    public void OrderType_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        var name = "Suporte & Manuten��o";
        var description = "Suporte t�cnico & manuten��o de sistemas";

        // Act
        var orderType = OrderType.Create(name, description, _dateTimeProvider);

        // Assert
        orderType.Name.Should().Be(name);
        orderType.Description.Should().Be(description);
        orderType.IsValid().Should().BeTrue();
    }

    [Fact]
    public void OrderType_ShouldInheritFromEntity()
    {
        // Arrange & Act
        var orderType = OrderType.Create("Test Type", "Test Description", _dateTimeProvider);

        // Assert
        orderType.Should().BeAssignableTo<EChamado.Shared.Shared.Entity>();
    }

    [Theory]
    [InlineData("TIPO MAI�SCULO", "DESCRI��O MAI�SCULA")]
    [InlineData("tipo min�sculo", "descri��o min�scula")]
    [InlineData("Tipo Misto", "Descri��o mista")]
    public void OrderType_ShouldPreserveCasing(string name, string description)
    {
        // Act
        var orderType = OrderType.Create(name, description, _dateTimeProvider);

        // Assert
        orderType.Name.Should().Be(name);
        orderType.Description.Should().Be(description);
    }

    [Fact]
    public void OrderType_WithBoundaryLengths_ShouldWork()
    {
        // Arrange
        var name = new string('A', 100); // Assumindo limite de 100
        var description = new string('B', 500); // Assumindo limite de 500

        // Act
        var orderType = OrderType.Create(name, description, _dateTimeProvider);

        // Assert
        orderType.Name.Should().Be(name);
        orderType.Description.Should().Be(description);
        orderType.IsValid().Should().BeTrue();
    }
}