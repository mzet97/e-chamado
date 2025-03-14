using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Domains.Orders.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace EChamado.Server.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions options, ILoggerFactory loggerFactory) : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    Guid,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    ApplicationRoleClaim,
    ApplicationUserToken>(options)
{

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (loggerFactory != null)
        {
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }

        base.OnConfiguring(optionsBuilder);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

        modelBuilder.UseOpenIddict();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("CreatedAt") != null))
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                entry.Property("CreatedAt").IsModified = false;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
    public DbSet<ApplicationRole> ApplicationRoles { get; set; } = null!;
    public DbSet<ApplicationUserClaim> ApplicationUserClains { get; set; } = null!;
    public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; } = null!;
    public DbSet<ApplicationRoleClaim> ApplicationRoleClains { get; set; } = null!;
    public DbSet<ApplicationUserToken> ApplicationUserTokens { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<SubCategory> SubCategories { get; set; } = null!;
    public DbSet<Department> Departments { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<StatusType> StatusTypes { get; set; } = null!;
    public DbSet<OrderType> OrderTypes { get; set; } = null!;
}