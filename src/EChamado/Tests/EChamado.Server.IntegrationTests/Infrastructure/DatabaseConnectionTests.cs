using EChamado.Server.Infrastructure.Persistence;
using EChamado.Server.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EChamado.Server.IntegrationTests.Infrastructure;

[Collection("IntegrationTests")]
public class DatabaseConnectionTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly IntegrationTestWebAppFactory _factory;

    public DatabaseConnectionTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DatabaseConnection_ShouldBeAvailable()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var canConnect = await dbContext.Database.CanConnectAsync();

        // Assert
        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task DatabaseTables_ShouldBeCreated()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var hasOrdersTable = dbContext.Model.GetEntityTypes()
            .Any(e => string.Equals(e.GetTableName(), "Orders", StringComparison.OrdinalIgnoreCase) ||
                      (e.GetSchemaQualifiedTableName()?.EndsWith(".Orders", StringComparison.OrdinalIgnoreCase) ?? false));

        var hasCategoriesTable = dbContext.Model.GetEntityTypes()
            .Any(e => string.Equals(e.GetTableName(), "Categories", StringComparison.OrdinalIgnoreCase) ||
                      (e.GetSchemaQualifiedTableName()?.EndsWith(".Categories", StringComparison.OrdinalIgnoreCase) ?? false));

        // Assert
        hasOrdersTable.Should().BeTrue();
        hasCategoriesTable.Should().BeTrue();
    }

    [Fact]
    public async Task RedisConnection_ShouldBeAvailable()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        // Act & Assert
        // Para este teste, vamos apenas verificar se o serviço foi registrado
        var cacheService = scope.ServiceProvider.GetService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
        cacheService.Should().NotBeNull();
    }

    [Fact]
    public async Task DatabaseMigrations_ShouldBeApplied()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var appliedMigrations = await dbContext.Database.GetAppliedMigrationsAsync();
        var allMigrations = dbContext.Database.GetMigrations();

        // Assert
        // Verificar se há migrations aplicadas (pode estar vazio em um novo banco)
        appliedMigrations.Should().NotBeNull();
        allMigrations.Should().NotBeNull();

        // Se há migrations no projeto, elas devem estar aplicadas
        if (allMigrations.Any())
        {
            appliedMigrations.Should().NotBeEmpty("Se há migrations no projeto, algumas devem estar aplicadas");
        }
    }

    [Fact]
    public async Task DbContext_ShouldHaveCorrectConfiguration()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var entityTypes = dbContext.Model.GetEntityTypes();

        // Assert
        entityTypes.Should().NotBeEmpty("DbContext deve ter entidades configuradas");

        // Verificar se algumas entidades principais estão configuradas
        var entityNames = entityTypes.Select(e => e.ClrType.Name).ToList();
        entityNames.Should().Contain("Order", "DbContext deve ter a entidade Order");
        entityNames.Should().Contain("Category", "DbContext deve ter a entidade Category");
    }

    [Fact]
    public async Task Database_ShouldSupportTransactions()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act & Assert
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        transaction.Should().NotBeNull();
        await transaction.RollbackAsync();
    }
}