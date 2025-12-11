//using EChamado.Server.Domain.Domains.Orders.Entities;
//using EChamado.Shared.Services;
//using EChamado.Server.UnitTests.Common.Base;
//using FluentAssertions;
//using Xunit;

//namespace EChamado.Server.UnitTests.Domain.Entities;

//public class SubCategoryTests : UnitTestBase
//{
//    private static readonly IDateTimeProvider _dateTimeProvider = new MockDateTimeProvider();

//    private class MockDateTimeProvider : IDateTimeProvider
//    {
//        public DateTime Now => DateTime.Now;
//        public DateTime UtcNow => DateTime.UtcNow;
//        public DateTimeOffset OffsetNow => DateTimeOffset.Now;
//        public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
//    }
//    [Fact]
//    public void Create_WithValidData_ShouldCreateSubCategory()
//    {
//        // Arrange
//        var name = "Hardware";
//        var description = "Problemas relacionados a hardware";
//        var categoryId = Guid.NewGuid();

//        // Act
//        var subCategory = SubCategory.Create(name, description, categoryId, _dateTimeProvider);

//        // Assert
//        subCategory.Should().NotBeNull();
//        subCategory.Name.Should().Be(name);
//        subCategory.Description.Should().Be(description);
//        subCategory.CategoryId.Should().Be(categoryId);
//        subCategory.Id.Should().NotBe(Guid.Empty);
//        subCategory.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
//        subCategory.IsValid().Should().BeTrue();
//    }

//    [Fact]
//    public void Create_WithInvalidData_ShouldCreateInvalidSubCategory()
//    {
//        // Arrange
//        var name = ""; // Nome inv�lido
//        var description = "Descri��o v�lida";
//        var categoryId = Guid.NewGuid();

//        // Act
//        var subCategory = SubCategory.Create(name, description, categoryId, _dateTimeProvider);

//        // Assert
//        subCategory.Should().NotBeNull();
//        subCategory.IsValid().Should().BeFalse();
//        subCategory.GetErrors().Should().NotBeEmpty();
//    }

//    [Fact]
//    public void Update_WithValidData_ShouldUpdateSubCategory()
//    {
//        // Arrange
//        var subCategory = SubCategory.Create("Original", "Original Description", Guid.NewGuid(), _dateTimeProvider);
//        var newName = "Updated Name";
//        var newDescription = "Updated Description";
//        var newCategoryId = Guid.NewGuid();

//        // Act
//        subCategory.Update(newName, newDescription, newCategoryId, _dateTimeProvider);

//        // Assert
//        subCategory.Name.Should().Be(newName);
//        subCategory.Description.Should().Be(newDescription);
//        subCategory.CategoryId.Should().Be(newCategoryId);
//        subCategory.UpdatedAt.Should().NotBeNull();
//        subCategory.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
//    }

//    [Theory]
//    [InlineData("Software", "Problemas de software")]
//    [InlineData("Rede", "Problemas de conectividade")]
//    [InlineData("Impressora", "Problemas com impress�o")]
//    [InlineData("Monitor", "Problemas de v�deo")]
//    public void Create_WithDifferentValidInputs_ShouldCreateValidSubCategory(string name, string description)
//    {
//        // Arrange
//        var categoryId = Guid.NewGuid();

//        // Act
//        var subCategory = SubCategory.Create(name, description, categoryId, _dateTimeProvider);

//        // Assert
//        subCategory.Should().NotBeNull();
//        subCategory.Name.Should().Be(name);
//        subCategory.Description.Should().Be(description);
//        subCategory.CategoryId.Should().Be(categoryId);
//        subCategory.IsValid().Should().BeTrue();
//    }

//    [Theory]
//    [InlineData("", "Description")]
//    [InlineData("   ", "Description")]
//    [InlineData("\t", "Description")]
//    public void Create_WithInvalidName_ShouldCreateInvalidSubCategory(string invalidName, string description)
//    {
//        // Arrange
//        var categoryId = Guid.NewGuid();

//        // Act
//        var subCategory = SubCategory.Create(invalidName, description, categoryId, _dateTimeProvider);

//        // Assert
//        subCategory.IsValid().Should().BeFalse();
//        subCategory.GetErrors().Should().NotBeEmpty();
//    }

//    [Theory]
//    [InlineData("Name", "")]
//    [InlineData("Name", "   ")]
//    [InlineData("Name", "\t")]
//    public void Create_WithInvalidDescription_ShouldCreateInvalidSubCategory(string name, string invalidDescription)
//    {
//        // Arrange
//        var categoryId = Guid.NewGuid();

//        // Act
//        var subCategory = SubCategory.Create(name, invalidDescription, categoryId, _dateTimeProvider);

//        // Assert
//        subCategory.IsValid().Should().BeFalse();
//        subCategory.GetErrors().Should().NotBeEmpty();
//    }

//    [Fact]
//    public void Create_WithEmptyCategoryId_ShouldCreateInvalidSubCategory()
//    {
//        // Arrange
//        var name = "Valid Name";
//        var description = "Valid Description";
//        var categoryId = Guid.Empty;

//        // Act
//        var subCategory = SubCategory.Create(name, description, categoryId, _dateTimeProvider);

//        // Assert
//        subCategory.IsValid().Should().BeFalse();
//        subCategory.GetErrors().Should().NotBeEmpty();
//    }

//    [Fact]
//    public void SubCategory_ShouldHaveReadOnlyProperties()
//    {
//        // Arrange
//        var subCategory = SubCategory.Create("Test", "Description", Guid.NewGuid(), _dateTimeProvider);

//        // Act & Assert
//        var nameProperty = typeof(SubCategory).GetProperty(nameof(SubCategory.Name));
//        var descriptionProperty = typeof(SubCategory).GetProperty(nameof(SubCategory.Description));
//        var categoryIdProperty = typeof(SubCategory).GetProperty(nameof(SubCategory.CategoryId));

//        nameProperty!.SetMethod.Should().NotBeNull();
//        nameProperty.SetMethod!.IsPublic.Should().BeFalse();

//        descriptionProperty!.SetMethod.Should().NotBeNull();
//        descriptionProperty.SetMethod!.IsPublic.Should().BeFalse();

//        categoryIdProperty!.SetMethod.Should().NotBeNull();
//        categoryIdProperty.SetMethod!.IsPublic.Should().BeFalse();
//    }

//    [Fact]
//    public void Create_MultipleSubCategories_ShouldHaveUniqueIds()
//    {
//        // Arrange
//        var categoryId = Guid.NewGuid();

//        // Act
//        var subCategory1 = SubCategory.Create("Sub1", "Description1", categoryId, _dateTimeProvider);
//        var subCategory2 = SubCategory.Create("Sub2", "Description2", categoryId, _dateTimeProvider);

//        // Assert
//        subCategory1.Id.Should().NotBe(subCategory2.Id);
//        subCategory1.CategoryId.Should().Be(subCategory2.CategoryId);
//    }

//    [Fact]
//    public void Update_WithInvalidData_ShouldMakeSubCategoryInvalid()
//    {
//        // Arrange
//        var subCategory = SubCategory.Create("Valid", "Valid Description", Guid.NewGuid(), _dateTimeProvider);

//        // Act
//        subCategory.Update("", "Description", Guid.NewGuid(), _dateTimeProvider); // Nome vazio

//        // Assert
//        subCategory.IsValid().Should().BeFalse();
//        subCategory.GetErrors().Should().NotBeEmpty();
//    }

//    [Fact]
//    public void SubCategory_ShouldInheritFromEntity()
//    {
//        // Arrange & Act
//        var subCategory = SubCategory.Create("Test", "Description", Guid.NewGuid(), _dateTimeProvider);

//        // Assert
//        subCategory.Should().BeAssignableTo<Shared.Domain.Entity>();
//    }

//    [Fact]
//    public void SubCategory_WithBoundaryLengths_ShouldWork()
//    {
//        // Arrange
//        var name = new string('A', 100); // Assumindo limite de 100
//        var description = new string('B', 500); // Assumindo limite de 500
//        var categoryId = Guid.NewGuid();

//        // Act
//        var subCategory = SubCategory.Create(name, description, categoryId, _dateTimeProvider);

//        // Assert
//        subCategory.Name.Should().Be(name);
//        subCategory.Description.Should().Be(description);
//        subCategory.IsValid().Should().BeTrue();
//    }

//    [Fact]
//    public void Update_MultipleTimes_ShouldUpdateTimestamp()
//    {
//        // Arrange
//        var subCategory = SubCategory.Create("Original", "Original Description", Guid.NewGuid(), _dateTimeProvider);

//        // Act
//        subCategory.Update("Name1", "Description1", Guid.NewGuid(), _dateTimeProvider);
//        var firstUpdate = subCategory.UpdatedAt;

//        Thread.Sleep(100);

//        subCategory.Update("Name2", "Description2", Guid.NewGuid(), _dateTimeProvider);
//        var secondUpdate = subCategory.UpdatedAt;

//        // Assert
//        secondUpdate.Should().BeAfter(firstUpdate!.Value);
//        subCategory.Name.Should().Be("Name2");
//    }

//    [Fact]
//    public void Create_ShouldSetDefaultProperties()
//    {
//        // Arrange & Act
//        var subCategory = SubCategory.Create("Test", "Description", Guid.NewGuid(), _dateTimeProvider);

//        // Assert
//        subCategory.Id.Should().NotBe(Guid.Empty);
//        subCategory.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
//        subCategory.UpdatedAt.Should().BeNull();
//        subCategory.DeletedAt.Should().BeNull();
//        subCategory.IsDeleted.Should().BeFalse();
//    }
//}