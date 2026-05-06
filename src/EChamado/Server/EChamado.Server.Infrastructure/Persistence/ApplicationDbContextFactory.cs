using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // This is used by the EF Core CLI tools to create a DbContext instance
        // for design-time operations like migrations.

        // Define o caminho para o projeto Server onde est� o appsettings.json
        var serverProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "EChamado.Server");

        // Verifica se o diret�rio existe, caso contr�rio tenta o diret�rio atual
        if (!Directory.Exists(serverProjectPath))
        {
            serverProjectPath = Directory.GetCurrentDirectory();
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(serverProjectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Obt�m a connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Fallback para connection string padr�o
            connectionString = "Host=localhost;Port=5432;Pooling=true;Database=e-chamado;User Id=app;Password=CHANGEME;";
        }

        // Obt�m o provider de banco de dados (padr�o: Postgres)
        var databaseProvider = configuration.GetValue<string>("DatabaseProvider") ?? "Postgres";

        // Configura o LoggerFactory para o design time (sem usar 'using' para n�o descartar)
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        optionsBuilder.UseLoggerFactory(loggerFactory);

        // Configura o provider baseado na configura��o
        if (string.Equals(databaseProvider, "Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseSqlite(connectionString);
        }
        else
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        // Habilita detalhamento de erros e dados sens�veis em modo desenvolvimento
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        if (environment == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        return new ApplicationDbContext(optionsBuilder.Options, loggerFactory);
    }
}
