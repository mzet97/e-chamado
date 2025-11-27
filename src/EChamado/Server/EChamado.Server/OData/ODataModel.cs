using EChamado.Server.Domain.Domains.Orders;
using EChamado.Server.Domain.Domains.Orders.Entities;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace EChamado.Server.OData;

/// <summary>
/// Configuração centralizada do modelo EDM (Entity Data Model) para OData
/// </summary>
public static class ODataModel
{
    public static IEdmModel Model { get; } = GetEdmModel();

    private static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();

        // Configurar Orders
        ConfigureOrders(builder);

        // Configurar Categories
        ConfigureCategories(builder);

        // Configurar SubCategories
        ConfigureSubCategories(builder);

        // Configurar Departments
        ConfigureDepartments(builder);

        // Configurar OrderTypes
        ConfigureOrderTypes(builder);

        // Configurar StatusTypes
        ConfigureStatusTypes(builder);

        // Configurar Comments
        ConfigureComments(builder);

        return builder.GetEdmModel();
    }

    private static void ConfigureOrders(ODataConventionModelBuilder builder)
    {
        var orders = builder.EntitySet<Order>("Orders");
        orders.EntityType.HasKey(o => o.Id);

        // Properties
        orders.EntityType.Property(o => o.Title);
        orders.EntityType.Property(o => o.Description);
        orders.EntityType.Property(o => o.Evaluation);
        orders.EntityType.Property(o => o.OpeningDate);
        orders.EntityType.Property(o => o.ClosingDate);
        orders.EntityType.Property(o => o.DueDate);
        orders.EntityType.Property(o => o.StatusId);
        orders.EntityType.Property(o => o.TypeId);
        orders.EntityType.Property(o => o.RequestingUserId);
        orders.EntityType.Property(o => o.RequestingUserEmail);
        orders.EntityType.Property(o => o.ResponsibleUserId);
        orders.EntityType.Property(o => o.ResponsibleUserEmail);
        orders.EntityType.Property(o => o.CategoryId);
        orders.EntityType.Property(o => o.SubCategoryId);
        orders.EntityType.Property(o => o.DepartmentId);
        orders.EntityType.Property(o => o.CreatedAt);
        orders.EntityType.Property(o => o.UpdatedAt);
        orders.EntityType.Property(o => o.DeletedAt);
        orders.EntityType.Property(o => o.IsDeleted);

        // Relationships
        orders.EntityType.HasRequired(o => o.Status);
        orders.EntityType.HasRequired(o => o.Type);
        orders.EntityType.HasRequired(o => o.Category);
        orders.EntityType.HasOptional(o => o.SubCategory);
        orders.EntityType.HasRequired(o => o.Department);

        // Ignore domain events
        orders.EntityType.Ignore(o => o.Events);
    }

    private static void ConfigureCategories(ODataConventionModelBuilder builder)
    {
        var categories = builder.EntitySet<Category>("Categories");
        categories.EntityType.HasKey(c => c.Id);

        // Properties
        categories.EntityType.Property(c => c.Name);
        categories.EntityType.Property(c => c.Description);
        categories.EntityType.Property(c => c.CreatedAt);
        categories.EntityType.Property(c => c.UpdatedAt);
        categories.EntityType.Property(c => c.DeletedAt);
        categories.EntityType.Property(c => c.IsDeleted);

        // Relationships
        categories.EntityType.HasMany(c => c.SubCategories);

        // Ignore domain events
        categories.EntityType.Ignore(c => c.Events);
    }

    private static void ConfigureSubCategories(ODataConventionModelBuilder builder)
    {
        var subCategories = builder.EntitySet<SubCategory>("SubCategories");
        subCategories.EntityType.HasKey(sc => sc.Id);

        // Properties
        subCategories.EntityType.Property(sc => sc.Name);
        subCategories.EntityType.Property(sc => sc.Description);
        subCategories.EntityType.Property(sc => sc.CategoryId);
        subCategories.EntityType.Property(sc => sc.CreatedAt);
        subCategories.EntityType.Property(sc => sc.UpdatedAt);
        subCategories.EntityType.Property(sc => sc.DeletedAt);
        subCategories.EntityType.Property(sc => sc.IsDeleted);

        // Relationships
        subCategories.EntityType.HasRequired(sc => sc.Category);

        // Ignore domain events
        subCategories.EntityType.Ignore(sc => sc.Events);
    }

    private static void ConfigureDepartments(ODataConventionModelBuilder builder)
    {
        var departments = builder.EntitySet<Department>("Departments");
        departments.EntityType.HasKey(d => d.Id);

        // Properties
        departments.EntityType.Property(d => d.Name);
        departments.EntityType.Property(d => d.Description);
        departments.EntityType.Property(d => d.CreatedAt);
        departments.EntityType.Property(d => d.UpdatedAt);
        departments.EntityType.Property(d => d.DeletedAt);
        departments.EntityType.Property(d => d.IsDeleted);

        // Ignore domain events
        departments.EntityType.Ignore(d => d.Events);
    }

    private static void ConfigureOrderTypes(ODataConventionModelBuilder builder)
    {
        var orderTypes = builder.EntitySet<OrderType>("OrderTypes");
        orderTypes.EntityType.HasKey(ot => ot.Id);

        // Properties
        orderTypes.EntityType.Property(ot => ot.Name);
        orderTypes.EntityType.Property(ot => ot.Description);
        orderTypes.EntityType.Property(ot => ot.CreatedAt);
        orderTypes.EntityType.Property(ot => ot.UpdatedAt);
        orderTypes.EntityType.Property(ot => ot.DeletedAt);
        orderTypes.EntityType.Property(ot => ot.IsDeleted);

        // Ignore domain events
        orderTypes.EntityType.Ignore(ot => ot.Events);
    }

    private static void ConfigureStatusTypes(ODataConventionModelBuilder builder)
    {
        var statusTypes = builder.EntitySet<StatusType>("StatusTypes");
        statusTypes.EntityType.HasKey(st => st.Id);

        // Properties
        statusTypes.EntityType.Property(st => st.Name);
        statusTypes.EntityType.Property(st => st.Description);
        statusTypes.EntityType.Property(st => st.CreatedAt);
        statusTypes.EntityType.Property(st => st.UpdatedAt);
        statusTypes.EntityType.Property(st => st.DeletedAt);
        statusTypes.EntityType.Property(st => st.IsDeleted);

        // Ignore domain events
        statusTypes.EntityType.Ignore(st => st.Events);
    }

    private static void ConfigureComments(ODataConventionModelBuilder builder)
    {
        var comments = builder.EntitySet<Comment>("Comments");
        comments.EntityType.HasKey(c => c.Id);

        // Properties
        comments.EntityType.Property(c => c.Text);
        comments.EntityType.Property(c => c.OrderId);
        comments.EntityType.Property(c => c.UserId);
        comments.EntityType.Property(c => c.UserEmail);
        comments.EntityType.Property(c => c.CreatedAt);
        comments.EntityType.Property(c => c.UpdatedAt);
        comments.EntityType.Property(c => c.DeletedAt);
        comments.EntityType.Property(c => c.IsDeleted);

        // Relationships
        comments.EntityType.HasRequired(c => c.Order);

        // Ignore domain events
        comments.EntityType.Ignore(c => c.Events);
    }
}
