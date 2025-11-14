using EChamado.Server.Application.Users;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Server.Infrastructure.Users;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace EChamado.Tests.Server.IntegrationTests.Users;

public sealed class UserReadRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public UserReadRepositoryIntegrationTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .Options;
    }

    public async Task InitializeAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        context.Users.AddRange(
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "alpha@test.com",
                UserName = "alpha",
                FullName = "Alpha Test",
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            },
            new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = "beta@test.com",
                UserName = "beta",
                FullName = "Beta Test",
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow.AddDays(-1)
            });

        await context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task SearchAsync_DeveRetornarUsuariosOrdenados()
    {
        await using var context = CreateContext();
        var repository = new EfUserReadRepository(context);

        var result = await repository.SearchAsync(new UserSearchFilter
        {
            Page = 1,
            PageSize = 1,
            SortBy = "createdAt",
            SortDescending = true
        }, CancellationToken.None);

        Assert.Equal(1, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("beta@test.com", result.Items[0].Email);
    }

    private ApplicationDbContext CreateContext() => new(_options, NullLoggerFactory.Instance);
}
