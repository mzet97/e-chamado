using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Shared.Services;
using EChamado.Server.UnitTests.Common.Base;
using FluentAssertions;
using Xunit;

namespace EChamado.Server.UnitTests.Domain.Entities;

public class DepartmentTests : UnitTestBase
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
    public void Create_WithValidData_ShouldCreateDepartment()
    {
        // Arrange
        var name = "Tecnologia da Informa��o";
        var description = "Departamento respons�vel pela TI";

        // Act
        var department = Department.Create(name, description, _dateTimeProvider);

        // Assert
        department.Should().NotBeNull();
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
        department.Id.Should().NotBe(Guid.Empty);
        department.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        department.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithInvalidData_ShouldCreateInvalidDepartment()
    {
        // Arrange
        var name = ""; // Nome inv�lido
        var description = "Descri��o v�lida";

        // Act
        var department = Department.Create(name, description, _dateTimeProvider);

        // Assert
        department.Should().NotBeNull();
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateDepartment()
    {
        // Arrange
        var department = Department.Create("Original", "Original Description", _dateTimeProvider);
        var newName = "Updated Name";
        var newDescription = "Updated Description";

        // Act
        department.Update(newName, newDescription, _dateTimeProvider);

        // Assert
        department.Name.Should().Be(newName);
        department.Description.Should().Be(newDescription);
        department.UpdatedAt.Should().NotBeNull();
        department.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("Recursos Humanos", "Departamento de RH")]
    [InlineData("Financeiro", "Departamento financeiro")]
    [InlineData("Vendas", "Departamento de vendas")]
    [InlineData("Marketing", "Departamento de marketing")]
    [InlineData("Opera��es", "Departamento operacional")]
    public void Create_WithDifferentValidInputs_ShouldCreateValidDepartment(string name, string description)
    {
        // Act
        var department = Department.Create(name, description, _dateTimeProvider);

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
        var department = Department.Create(invalidName, description, _dateTimeProvider);

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
        var department = Department.Create(name, invalidDescription, _dateTimeProvider);

        // Assert
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Department_ShouldHaveReadOnlyProperties()
    {
        // Arrange
        var department = Department.Create("Test Department", "Test Description", _dateTimeProvider);

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
        var department1 = Department.Create("Department 1", "Description 1", _dateTimeProvider);
        var department2 = Department.Create("Department 2", "Description 2", _dateTimeProvider);

        // Assert
        department1.Id.Should().NotBe(department2.Id);
    }

    [Fact]
    public void Update_WithInvalidData_ShouldMakeDepartmentInvalid()
    {
        // Arrange
        var department = Department.Create("Valid Name", "Valid Description", _dateTimeProvider);

        // Act
        department.Update("", "Description", _dateTimeProvider); // Nome vazio

        // Assert
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }

    [Fact]
    public void Department_ShouldInheritFromEntity()
    {
        // Arrange & Act
        var department = Department.Create("Test Department", "Test Description", _dateTimeProvider);

        // Assert
        department.Should().BeAssignableTo<Shared.Domain.Entity>();
    }

    [Fact]
    public void Department_WithBoundaryLengths_ShouldWork()
    {
        // Arrange
        var name = new string('A', 100); // Assumindo limite de 100
        var description = new string('B', 500); // Assumindo limite de 500

        // Act
        var department = Department.Create(name, description, _dateTimeProvider);

        // Assert
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
        department.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Update_MultipleTimes_ShouldUpdateTimestamp()
    {
        // Arrange
        var department = Department.Create("Original", "Original Description", _dateTimeProvider);

        // Act
        department.Update("Name1", "Description1", _dateTimeProvider);
        var firstUpdate = department.UpdatedAt;

        Thread.Sleep(100);

        department.Update("Name2", "Description2", _dateTimeProvider);
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
        var department = Department.Create("Test Department", "Test Description", _dateTimeProvider);

        // Assert
        department.Id.Should().NotBe(Guid.Empty);
        department.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        department.UpdatedAt.Should().BeNull();
        department.DeletedAt.Should().BeNull();
        department.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Department_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        var name = "TI & Inova��o";
        var description = "Tecnologia da Informa��o & Inova��o Digital";

        // Act
        var department = Department.Create(name, description, _dateTimeProvider);

        // Assert
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
        department.IsValid().Should().BeTrue();
    }

    [Theory]
    [InlineData("DEPARTMENT MAI�SCULO", "DESCRI��O MAI�SCULA")]
    [InlineData("department min�sculo", "descri��o min�scula")]
    [InlineData("Department Misto", "Descri��o mista")]
    public void Department_ShouldPreserveCasing(string name, string description)
    {
        // Act
        var department = Department.Create(name, description, _dateTimeProvider);

        // Assert
        department.Name.Should().Be(name);
        department.Description.Should().Be(description);
    }

    [Fact]
    public void Department_WithUnicodeCharacters_ShouldWork()
    {
        // Arrange
        var name = "Departamento de Inova��o ??";
        var description = "Departamento respons�vel pela inova��o e tecnologia ??";

        // Act
        var department = Department.Create(name, description, _dateTimeProvider);

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
        var department = Department.Create(name, description, _dateTimeProvider);

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
        var department = Department.Create(name, description, _dateTimeProvider);

        // Assert
        department.IsValid().Should().BeFalse();
        department.GetErrors().Should().NotBeEmpty();
    }
}