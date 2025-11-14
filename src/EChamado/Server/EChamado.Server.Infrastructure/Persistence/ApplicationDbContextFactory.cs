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
        
        // Define o caminho para o projeto Server onde está o appsettings.json
        var serverProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "EChamado.Server");
        
        // Verifica se o diretório existe, caso contrário tenta o diretório atual
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
        
        // Obtém a connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // Fallback para connection string padrão
            connectionString = "Host=192.168.31.52;Port=5432;Pooling=true;Database=e-chamado;User Id=app;Password=Admin@123;";
        }

        // Obtém o provider de banco de dados (padrão: Postgres)
        var databaseProvider = configuration.GetValue<string>("DatabaseProvider") ?? "Postgres";

        // Configura o LoggerFactory para o design time (sem usar 'using' para não descartar)
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        optionsBuilder.UseLoggerFactory(loggerFactory);

        // Configura o provider baseado na configuração
        if (string.Equals(databaseProvider, "Sqlite", StringComparison.OrdinalIgnoreCase))
        {
            optionsBuilder.UseSqlite(connectionString);
        }
        else
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        // Habilita detalhamento de erros e dados sensíveis em modo desenvolvimento
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        if (environment == "Development")
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        }

        return new ApplicationDbContext(optionsBuilder.Options, loggerFactory);
    }
}
