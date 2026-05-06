using System;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Infrastructure.Configuration;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        var skipInitializer = Environment.GetEnvironmentVariable("SKIP_DB_INITIALIZER");
        if (string.Equals(skipInitializer, "true", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            logger.LogInformation("Starting database migration...");

            // Aplica migrations pendentes
            await context.Database.MigrateAsync();

            logger.LogInformation("Database migration completed successfully.");

            // Seed de dados iniciais
            await SeedDataAsync(context, userManager, roleManager, logger);

            logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private static async Task SeedDataAsync(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ILogger logger)
    {
        // Seed Roles
        var roles = new[] { "Admin", "User", "Support" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole { Name = roleName };
                await roleManager.CreateAsync(role);
                logger.LogInformation($"Role '{roleName}' created successfully.");
            }
        }

        // Seed Admin User
        const string adminEmail = "admin@echamado.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                FullName = "Administrador",
                Email = adminEmail,
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            var adminPassword = Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD") ?? "Admin@123";
            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation($"Admin user created successfully with email: {adminEmail}");
            }
            else
            {
                logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // Seed Test User
        const string testEmail = "user@echamado.com";
        var testUser = await userManager.FindByEmailAsync(testEmail);

        if (testUser == null)
        {
            testUser = new ApplicationUser
            {
                UserName = "testuser",
                FullName = "Usuário de Teste",
                Email = testEmail,
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            var testPassword = Environment.GetEnvironmentVariable("SEED_TEST_PASSWORD") ?? "User@123";
            var result = await userManager.CreateAsync(testUser, testPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(testUser, "User");
                logger.LogInformation($"Test user created successfully with email: {testEmail}");
            }
            else
            {
                logger.LogError($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        await context.SaveChangesAsync();
    }
}

