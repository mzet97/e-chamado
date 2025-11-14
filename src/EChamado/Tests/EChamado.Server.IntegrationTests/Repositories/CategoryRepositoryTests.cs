using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Server.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EChamado.Server.IntegrationTests.Repositories;

[Collection("IntegrationTests")]
public class CategoryRepositoryTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;

    public CategoryRepositoryTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AddAsync_WithValidCategory_ShouldPersistToDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var category = Category.Create("Test Category", "Test Description");

        // Act
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        // Assert
        var savedCategory = await dbContext.Categories.FindAsync(category.Id);
        savedCategory.Should().NotBeNull();
        savedCategory!.Name.Should().Be("Test Category");
        savedCategory.Description.Should().Be("Test Description");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingCategory_ShouldReturnCategory()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var category = Category.Create("Search Test Category", "Search Test Description");
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        // Act
        var foundCategory = await dbContext.Categories.FindAsync(category.Id);

        // Assert
        foundCategory.Should().NotBeNull();
        foundCategory!.Id.Should().Be(category.Id);
        foundCategory.Name.Should().Be("Search Test Category");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentCategory_ShouldReturnNull()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var nonExistentId = Guid.NewGuid();

        // Act
        var foundCategory = await dbContext.Categories.FindAsync(nonExistentId);

        // Assert
        foundCategory.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithModifiedCategory_ShouldPersistChanges()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var category = Category.Create("Original Name", "Original Description");
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        // Act
        category.Update("Updated Name", "Updated Description");
        dbContext.Categories.Update(category);
        await dbContext.SaveChangesAsync();

        // Assert
        var updatedCategory = await dbContext.Categories.FindAsync(category.Id);
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Name.Should().Be("Updated Name");
        updatedCategory.Description.Should().Be("Updated Description");
        updatedCategory.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCategoryFromDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var category = Category.Create("To Delete", "Will be deleted");
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync();

        // Act
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync();

        // Assert
        var deletedCategory = await dbContext.Categories.FindAsync(category.Id);
        deletedCategory.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCategories()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var category1 = Category.Create("Category 1", "Description 1");
        var category2 = Category.Create("Category 2", "Description 2");
        var category3 = Category.Create("Category 3", "Description 3");
        
        dbContext.Categories.AddRange(category1, category2, category3);
        await dbContext.SaveChangesAsync();

        // Act
        var allCategories = dbContext.Categories.ToList();

        // Assert
        allCategories.Should().HaveCountGreaterOrEqualTo(3);
        allCategories.Should().Contain(c => c.Name == "Category 1");
        allCategories.Should().Contain(c => c.Name == "Category 2");
        allCategories.Should().Contain(c => c.Name == "Category 3");
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleOperations_ShouldPersistAllChanges()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var categoryToAdd = Category.Create("New Category", "New Description");
        var categoryToUpdate = Category.Create("To Update", "Original");
        
        dbContext.Categories.Add(categoryToUpdate);
        await dbContext.SaveChangesAsync();

        // Act
        dbContext.Categories.Add(categoryToAdd);
        categoryToUpdate.Update("Updated", "Modified Description");
        dbContext.Categories.Update(categoryToUpdate);
        
        await dbContext.SaveChangesAsync();

        // Assert
        var addedCategory = await dbContext.Categories.FindAsync(categoryToAdd.Id);
        var updatedCategory = await dbContext.Categories.FindAsync(categoryToUpdate.Id);
        
        addedCategory.Should().NotBeNull();
        addedCategory!.Name.Should().Be("New Category");
        
        updatedCategory.Should().NotBeNull();
        updatedCategory!.Name.Should().Be("Updated");
        updatedCategory.Description.Should().Be("Modified Description");
    }
}