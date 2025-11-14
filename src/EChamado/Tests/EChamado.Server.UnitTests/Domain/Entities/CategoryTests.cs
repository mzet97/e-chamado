using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.UnitTests.Common.Base;
using EChamado.Server.UnitTests.Common.Builders;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class CategoryTests : UnitTestBase
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var name = "Categoria Teste";
        var description = "Descrição da categoria";

        // Act
        var category = Category.Create(name, description);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(name);
        category.Description.Should().Be(description);
        category.Id.Should().NotBe(Guid.Empty);
        category.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        category.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidCategory()
    {
        // Arrange
        var name = ""; // Nome inválido
        var description = "Descrição válida";

        // Act
        var category = Category.Create(name, description);

        // Assert
        category.Should().NotBeNull();
        category.IsValid().Should().BeFalse();
        category.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateCategory()
    {
        // Arrange
        var category = CategoryTestBuilder.Create()
            .WithValidData()
            .Build();
        
        var newName = "Nome Atualizado";
        var newDescription = "Descrição Atualizada";

        // Act
        category.Update(newName, newDescription);

        // Assert
        category.Name.Should().Be(newName);
        category.Description.Should().Be(newDescription);
        category.UpdatedAt.Should().NotBeNull();
        category.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Update_WithInvalidData_ShouldMakeCategoryInvalid()
    {
        // Arrange
        var category = CategoryTestBuilder.Create()
            .WithValidData()
            .Build();

        // Act
        category.Update("", "Descrição"); // Nome vazio

        // Assert
        category.IsValid().Should().BeFalse();
        category.GetErrors().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("AA", "B")] // Valores mínimos
    [InlineData("Categoria Normal", "Descrição normal")]  
    [InlineData("CATEGORIA MAIÚSCULA", "DESCRIÇÃO MAIÚSCULA")]
    [InlineData("categoria minúscula", "descrição minúscula")]
    public void Create_WithDifferentValidInputs_ShouldCreateValidCategory(string name, string description)
    {
        // Act
        var category = Category.Create(name, description);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(name);
        category.Description.Should().Be(description);
        category.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_ShouldSetDefaultProperties()
    {
        // Arrange
        var name = "Categoria Teste";
        var description = "Descrição da categoria";

        // Act
        var category = Category.Create(name, description);

        // Assert
        category.Id.Should().NotBe(Guid.Empty);
        category.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        category.UpdatedAt.Should().BeNull();
        category.DeletedAt.Should().BeNull();
        category.IsDeleted.Should().BeFalse();
        category.SubCategories.Should().NotBeNull();
        category.SubCategories.Should().BeEmpty();
    }

    [Fact]
    public void Create_ShouldTriggerValidation()
    {
        // Arrange & Act
        var validCategory = Category.Create("Nome Válido", "Descrição Válida");
        var invalidCategory = Category.Create("", "Descrição");

        // Assert
        validCategory.IsValid().Should().BeTrue();
        validCategory.GetErrors().Should().BeEmpty();

        invalidCategory.IsValid().Should().BeFalse();
        invalidCategory.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var category = CategoryTestBuilder.Create().WithValidData().Build();
        var firstUpdateTime = DateTime.Now;

        // Act
        category.Update("Nome 1", "Descrição 1");
        var firstUpdate = category.UpdatedAt;

        Thread.Sleep(100); // Pequena pausa para garantir timestamp diferente

        category.Update("Nome 2", "Descrição 2");
        var secondUpdate = category.UpdatedAt;

        // Assert
        firstUpdate.Should().NotBeNull();
        secondUpdate.Should().NotBeNull();
        secondUpdate.Should().BeAfter(firstUpdate!.Value);
        category.Name.Should().Be("Nome 2");
        category.Description.Should().Be("Descrição 2");
    }

    [Fact]
    public void Category_WithLongValidData_ShouldWork()
    {
        // Arrange
        var name = new string('A', 100); // No limite
        var description = new string('B', 500); // No limite

        // Act
        var category = Category.Create(name, description);

        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(name);
        category.Description.Should().Be(description);
        category.IsValid().Should().BeTrue();
    }
}