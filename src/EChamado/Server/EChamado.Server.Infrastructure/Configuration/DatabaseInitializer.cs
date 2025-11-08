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
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation($"Admin user created successfully with email: {adminEmail}");
                logger.LogInformation("Default admin credentials: admin@echamado.com / Admin@123");
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
                Email = testEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(testUser, "User@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(testUser, "User");
                logger.LogInformation($"Test user created successfully with email: {testEmail}");
                logger.LogInformation("Default test user credentials: user@echamado.com / User@123");
            }
            else
            {
                logger.LogError($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        await context.SaveChangesAsync();
    }
}
