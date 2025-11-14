using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.UnitTests.Common.Base;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class OrderTypeTests : UnitTestBase
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrderType()
    {
        // Arrange
        var name = "Suporte Técnico";
        var description = "Solicitações de suporte técnico";

        // Act
        var orderType = OrderType.Create(name, description);

        // Assert
        orderType.Should().NotBeNull();
        orderType.Name.Should().Be(name);
        orderType.Description.Should().Be(description);
        orderType.Id.Should().NotBe(Guid.Empty);
        orderType.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        orderType.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidOrderType()
    {
        // Arrange
        var name = ""; // Nome inválido
        var description = "Descrição válida";

        // Act
        var orderType = OrderType.Create(name, description);

        // Assert
        orderType.Should().NotBeNull();
        orderType.IsValid().Should().BeFalse();
        orderType.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateOrderType()
    {
        // Arrange
        var orderType = OrderType.Create("Tipo Original", "Descrição Original");
        var newName = "Tipo Atualizado";
        var newDescription = "Descrição Atualizada";

        // Act
        orderType.Update(newName, newDescription);

        // Assert
        orderType.Name.Should().Be(newName);
        orderType.Description.Should().Be(newDescription);
        orderType.UpdatedAt.Should().NotBeNull();
        orderType.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("Suporte Técnico", "Questões relacionadas a TI")]
    [InlineData("Recursos Humanos", "Questões de RH")]
    [InlineData("Financeiro", "Questões financeiras")]
    [InlineData("Infraestrutura", "Questões de infraestrutura")]
    public void Create_WithDifferentValidInputs_ShouldCreateValidOrderType(string name, string description)
    {
        // Act
        var orderType = OrderType.Create(name, description);

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
        var description = "Descrição do tipo";

        // Act
        var orderType = OrderType.Create(name, description);

        // Assert
        orderType.Id.Should().NotBe(Guid.Empty);
        orderType.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        orderType.UpdatedAt.Should().BeNull();
        orderType.DeletedAt.Should().BeNull();
        orderType.IsDeleted.Should().BeFalse();
    }

    [Theory]
    [InlineData("", "Descrição válida")]
    [InlineData("   ", "Descrição válida")]
    [InlineData("\t", "Descrição válida")]
    public void Create_WithInvalidName_ShouldCreateInvalidOrderType(string invalidName, string description)
    {
        // Act
        var orderType = OrderType.Create(invalidName, description);

        // Assert
        orderType.IsValid().Should().BeFalse();
        orderType.GetErrors().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Nome válido", "")]
    [InlineData("Nome válido", "   ")]
    [InlineData("Nome válido", "\t")]
    public void Create_WithInvalidDescription_ShouldCreateInvalidOrderType(string name, string invalidDescription)
    {
        // Act
        var orderType = OrderType.Create(name, invalidDescription);

        // Assert
        orderType.IsValid().Should().BeFalse();
        orderType.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void OrderType_ShouldHaveReadOnlyProperties()
    {
        // Arrange
        var orderType = OrderType.Create("Test Type", "Test Description");

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
        var orderType1 = OrderType.Create("Tipo 1", "Descrição 1");
        var orderType2 = OrderType.Create("Tipo 2", "Descrição 2");

        // Assert
        orderType1.Id.Should().NotBe(orderType2.Id);
    }

    [Fact]
    public void Update_WithInvalidData_ShouldMakeOrderTypeInvalid()
    {
        // Arrange
        var orderType = OrderType.Create("Tipo Válido", "Descrição Válida");

        // Act
        orderType.Update("", "Descrição"); // Nome vazio

        // Assert
        orderType.IsValid().Should().BeFalse();
        orderType.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var orderType = OrderType.Create("Tipo Original", "Descrição Original");

        // Act
        orderType.Update("Tipo 1", "Descrição 1");
        var firstUpdate = orderType.UpdatedAt;

        Thread.Sleep(100);

        orderType.Update("Tipo 2", "Descrição 2");
        var secondUpdate = orderType.UpdatedAt;

        // Assert
        secondUpdate.Should().BeAfter(firstUpdate!.Value);
        orderType.Name.Should().Be("Tipo 2");
        orderType.Description.Should().Be("Descrição 2");
    }

    [Fact]
    public void OrderType_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        var name = "Suporte & Manutenção";
        var description = "Suporte técnico & manutenção de sistemas";

        // Act
        var orderType = OrderType.Create(name, description);

        // Assert
        orderType.Name.Should().Be(name);
        orderType.Description.Should().Be(description);
        orderType.IsValid().Should().BeTrue();
    }

    [Fact]
    public void OrderType_ShouldInheritFromEntity()
    {
        // Arrange & Act
        var orderType = OrderType.Create("Test Type", "Test Description");

        // Assert
        orderType.Should().BeAssignableTo<EChamado.Shared.Shared.Entity>();
    }

    [Theory]
    [InlineData("TIPO MAIÚSCULO", "DESCRIÇÃO MAIÚSCULA")]
    [InlineData("tipo minúsculo", "descrição minúscula")]
    [InlineData("Tipo Misto", "Descrição mista")]
    public void OrderType_ShouldPreserveCasing(string name, string description)
    {
        // Act
        var orderType = OrderType.Create(name, description);

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
        var orderType = OrderType.Create(name, description);

        // Assert
        orderType.Name.Should().Be(name);
        orderType.Description.Should().Be(description);
        orderType.IsValid().Should().BeTrue();
    }
}