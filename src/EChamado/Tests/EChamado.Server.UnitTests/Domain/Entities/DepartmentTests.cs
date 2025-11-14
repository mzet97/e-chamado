using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.UnitTests.Common.Base;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class DepartmentTests : UnitTestBase
{
    [Fact]
    public void Create_WithValidData_ShouldCreateDepartment()
    {
        // Arrange
        var name = "Tecnologia da Informação";
        var description = "Departamento responsável pela TI";

        // Act
        var department = Department.Create(name, description);

        // Assert
        department.Should().NotBeNull();
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
        department.Id.Should().NotBe(Guid.Empty);
        department.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        department.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidDepartment()
    {
        // Arrange
        var name = ""; // Nome inválido
        var description = "Descrição válida";

        // Act
        var department = Department.Create(name, description);

        // Assert
        department.Should().NotBeNull();
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateDepartment()
    {
        // Arrange
        var department = Department.Create("Original", "Original Description");
        var newName = "Updated Name";
        var newDescription = "Updated Description";

        // Act
        department.Update(newName, newDescription);

        // Assert
        department.Name.Should().Be(newName);
        department.Description.Should().Be(newDescription);
        department.UpdatedAt.Should().NotBeNull();
        department.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("Recursos Humanos", "Departamento de RH")]
    [InlineData("Financeiro", "Departamento financeiro")]
    [InlineData("Vendas", "Departamento de vendas")]
    [InlineData("Marketing", "Departamento de marketing")]
    [InlineData("Operações", "Departamento operacional")]
    public void Create_WithDifferentValidInputs_ShouldCreateValidDepartment(string name, string description)
    {
        // Act
        var department = Department.Create(name, description);

        // Assert
        department.Should().NotBeNull();
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
        department.IsValid().Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Description")]
    [InlineData("   ", "Description")]
    [InlineData("\t", "Description")]
    [InlineData("\n", "Description")]
    public void Create_WithInvalidName_ShouldCreateInvalidDepartment(string invalidName, string description)
    {
        // Act
        var department = Department.Create(invalidName, description);

        // Assert
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("Name", "")]
    [InlineData("Name", "   ")]
    [InlineData("Name", "\t")]
    [InlineData("Name", "\n")]
    public void Create_WithInvalidDescription_ShouldCreateInvalidDepartment(string name, string invalidDescription)
    {
        // Act
        var department = Department.Create(name, invalidDescription);

        // Assert
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Department_ShouldHaveReadOnlyProperties()
    {
        // Arrange
        var department = Department.Create("Test Department", "Test Description");

        // Act & Assert
        var nameProperty = typeof(Department).GetProperty(nameof(Department.Name));
        var descriptionProperty = typeof(Department).GetProperty(nameof(Department.Description));

        nameProperty!.SetMethod.Should().NotBeNull();
        nameProperty.SetMethod!.IsPublic.Should().BeFalse();

        descriptionProperty!.SetMethod.Should().NotBeNull();
        descriptionProperty.SetMethod!.IsPublic.Should().BeFalse();
    }

    [Fact]
    public void Create_MultipleDepartments_ShouldHaveUniqueIds()
    {
        // Act
        var department1 = Department.Create("Department 1", "Description 1");
        var department2 = Department.Create("Department 2", "Description 2");

        // Assert
        department1.Id.Should().NotBe(department2.Id);
    }

    [Fact]
    public void Update_WithInvalidData_ShouldMakeDepartmentInvalid()
    {
        // Arrange
        var department = Department.Create("Valid Name", "Valid Description");

        // Act
        department.Update("", "Description"); // Nome vazio

        // Assert
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Department_ShouldInheritFromEntity()
    {
        // Arrange & Act
        var department = Department.Create("Test Department", "Test Description");

        // Assert
        department.Should().BeAssignableTo<EChamado.Shared.Shared.Entity>();
    }

    [Fact]
    public void Department_WithBoundaryLengths_ShouldWork()
    {
        // Arrange
        var name = new string('A', 100); // Assumindo limite de 100
        var description = new string('B', 500); // Assumindo limite de 500

        // Act
        var department = Department.Create(name, description);

        // Assert
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
        department.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Update_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var department = Department.Create("Original", "Original Description");

        // Act
        department.Update("Name1", "Description1");
        var firstUpdate = department.UpdatedAt;

        Thread.Sleep(100);

        department.Update("Name2", "Description2");
        var secondUpdate = department.UpdatedAt;

        // Assert
        secondUpdate.Should().BeAfter(firstUpdate!.Value);
        department.Name.Should().Be("Name2");
        department.Description.Should().Be("Description2");
    }

    [Fact]
    public void Create_ShouldSetDefaultProperties()
    {
        // Arrange & Act
        var department = Department.Create("Test Department", "Test Description");

        // Assert
        department.Id.Should().NotBe(Guid.Empty);
        department.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        department.UpdatedAt.Should().BeNull();
        department.DeletedAt.Should().BeNull();
        department.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Department_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        var name = "TI & Inovação";
        var description = "Tecnologia da Informação & Inovação Digital";

        // Act
        var department = Department.Create(name, description);

        // Assert
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
        department.IsValid().Should().BeTrue();
    }

    [Theory]
    [InlineData("DEPARTMENT MAIÚSCULO", "DESCRIÇÃO MAIÚSCULA")]
    [InlineData("department minúsculo", "descrição minúscula")]
    [InlineData("Department Misto", "Descrição mista")]
    public void Department_ShouldPreserveCasing(string name, string description)
    {
        // Act
        var department = Department.Create(name, description);

        // Assert
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
    }

    [Fact]
    public void Department_WithUnicodeCharacters_ShouldWork()
    {
        // Arrange
        var name = "Departamento de Inovação ??";
        var description = "Departamento responsável pela inovação e tecnologia ??";

        // Act
        var department = Department.Create(name, description);

        // Assert
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
        department.IsValid().Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(50)]
    [InlineData(100)]
    public void Department_WithDifferentNameLengths_ShouldWork(int length)
    {
        // Arrange
        var name = new string('A', length);
        var description = "Test Description";

        // Act
        var department = Department.Create(name, description);

        // Assert
        department.Name.Should().Be(name);
        department.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Department_WithTooShortName_ShouldBeInvalid()
    {
        // Arrange
        var name = "A";
        var description = "Descricao";

        // Act
        var department = Department.Create(name, description);

        // Assert
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }
}