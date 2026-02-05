using System;
using System.Collections.Generic;
using System.IO;
using EChamado.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EChamado.Server.IntegrationTests.Infrastructure;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly string _sqliteDbPath = Path.Combine(Path.GetTempPath(), $"echamado-tests-{Guid.NewGuid():N}.db");
    private readonly string? _originalConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    private readonly string? _originalDatabaseProvider = Environment.GetEnvironmentVariable("DatabaseProvider");
    private readonly string? _originalSkipInitializer = Environment.GetEnvironmentVariable("SKIP_DB_INITIALIZER");
    private static readonly object _dbLock = new();

    public IntegrationTestWebAppFactory()
    {
        if (File.Exists(_sqliteDbPath))
        {
            File.Delete(_sqliteDbPath);
        }

        var sqliteConnection = $"Data Source={_sqliteDbPath}";
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", sqliteConnection);
        Environment.SetEnvironmentVariable("DatabaseProvider", "Sqlite");
        Environment.SetEnvironmentVariable("SKIP_DB_INITIALIZER", "true");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_sqliteDbPath}",
                ["DatabaseProvider"] = "Sqlite",
                ["Redis:ConnectionString"] = "localhost",
                ["Redis:InstanceName"] = "EChamadoTests"
            };

            configBuilder.AddInMemoryCollection(overrides);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IDistributedCache));
            services.AddDistributedMemoryCache();

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            lock (_dbLock)
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultScheme = TestAuthHandler.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", _originalConnectionString);
            Environment.SetEnvironmentVariable("DatabaseProvider", _originalDatabaseProvider);
            Environment.SetEnvironmentVariable("SKIP_DB_INITIALIZER", _originalSkipInitializer);

            if (File.Exists(_sqliteDbPath))
            {
                File.Delete(_sqliteDbPath);
            }
        }

        base.Dispose(disposing);
    }
}
