namespace EChamado.Server.Application.Services.AI.Prompts;

/// <summary>
/// Prompt templates for converting Natural Language to Gridify queries
/// Customized for EChamado entities
/// </summary>
public static class GridifyPromptTemplates
{
    /// <summary>
    /// System message that defines the AI's role and behavior
    /// </summary>
    public static string SystemMessage => """
        You are a Gridify query expert for the EChamado system (a ticket/order management system).

        Your ONLY job is to convert natural language questions into valid Gridify query syntax.

        CRITICAL RULES:
        1. Return ONLY the Gridify query string - no explanations, no markdown, no code blocks
        2. Use ONLY the properties listed for each entity
        3. Use correct Gridify syntax operators
        4. Property names are case-sensitive
        5. String values must use single quotes, not double quotes
        6. Date comparisons should use format: yyyy-MM-dd or yyyy-MM-ddTHH:mm:ss

        GRIDIFY OPERATORS:
        - Comparison: =, !=, <, <=, >, >=
        - Logical: &, |, !
        - String: ^= (starts with), $= (ends with), *= (contains), !*= (not contains)
        - Date/Number ranges work with <, >, <=, >=

        ORDERING:
        - Use property name for ascending (e.g., "Name")
        - Use "-" prefix for descending (e.g., "-CreatedAt")
        - Multiple orderings: "Name, -CreatedAt"

        EXAMPLES:
        - "Name *= 'support'" → finds names containing "support"
        - "CreatedAt > 2024-01-01" → created after date
        - "IsDeleted = false & Priority = 1" → not deleted AND priority 1
        - "Name *= 'urgent' | StatusName *= 'open'" → name OR status match
        """;

    /// <summary>
    /// Get entity-specific prompt with metadata
    /// </summary>
    public static string GetEntityPrompt(string entityName, string naturalLanguageQuery)
    {
        var entityMetadata = GetEntityMetadata(entityName);

        return $"""
            ENTITY: {entityName}

            AVAILABLE PROPERTIES:
            {entityMetadata}

            NATURAL LANGUAGE QUERY:
            {naturalLanguageQuery}

            Convert this to a valid Gridify query using ONLY the properties listed above.
            Return ONLY the query string, nothing else.
            """;
    }

    /// <summary>
    /// Get metadata for a specific entity
    /// </summary>
    private static string GetEntityMetadata(string entityName)
    {
        return entityName.ToLowerInvariant() switch
        {
            "order" or "orders" => OrderMetadata,
            "category" or "categories" => CategoryMetadata,
            "subcategory" or "subcategories" => SubCategoryMetadata,
            "department" or "departments" => DepartmentMetadata,
            "ordertype" or "ordertypes" => OrderTypeMetadata,
            "statustype" or "statustypes" => StatusTypeMetadata,
            _ => throw new ArgumentException($"Unknown entity: {entityName}")
        };
    }

    #region Entity Metadata

    private static string OrderMetadata => """
        - Id (Guid): Unique identifier
        - Title (string): Order title
        - Description (string): Order description
        - StatusId (Guid): Status type ID
        - StatusName (string): Status type name (e.g., 'Aberto', 'Em Andamento', 'Fechado')
        - TypeId (Guid): Order type ID
        - TypeName (string): Order type name (e.g., 'Suporte', 'Requisição', 'Incidente')
        - DepartmentId (Guid): Department ID
        - DepartmentName (string): Department name
        - CategoryId (Guid): Category ID
        - CategoryName (string): Category name
        - SubCategoryId (Guid): Sub-category ID (nullable)
        - SubCategoryName (string): Sub-category name
        - Priority (int): Priority level (1=Low, 2=Medium, 3=High, 4=Critical)
        - OpeningDate (DateTime): When the order was opened
        - ClosingDate (DateTime?): When the order was closed (nullable)
        - DueDate (DateTime?): Expected completion date (nullable)
        - RequestingUserId (Guid): User who requested the order
        - RequestingUserEmail (string): Email of requesting user
        - AssignedUserId (Guid?): User assigned to the order (nullable)
        - AssignedUserEmail (string): Email of assigned user
        - CreatedAt (DateTime): Creation timestamp
        - UpdatedAt (DateTime?): Last update timestamp
        - IsDeleted (bool): Soft delete flag
        - DeletedAt (DateTime?): Deletion timestamp

        EXAMPLES:
        - "orders with high priority" → Priority = 3
        - "open orders" → StatusName *= 'Aberto'
        - "orders created this week" → CreatedAt >= 2024-11-25
        - "support tickets assigned to me" → TypeName *= 'Suporte' & AssignedUserEmail *= '@example.com'
        - "closed orders from IT department" → StatusName *= 'Fechado' & DepartmentName *= 'TI'
        """;

    private static string CategoryMetadata => """
        - Id (Guid): Unique identifier
        - Name (string): Category name
        - Description (string): Category description
        - Icon (string): Icon identifier
        - Color (string): Color code
        - IsActive (bool): Whether category is active
        - CreatedAt (DateTime): Creation timestamp
        - UpdatedAt (DateTime?): Last update timestamp
        - IsDeleted (bool): Soft delete flag

        EXAMPLES:
        - "active categories" → IsActive = true & IsDeleted = false
        - "categories with Hardware in name" → Name *= 'Hardware'
        - "categories created in 2024" → CreatedAt >= 2024-01-01 & CreatedAt < 2025-01-01
        """;

    private static string SubCategoryMetadata => """
        - Id (Guid): Unique identifier
        - Name (string): Sub-category name
        - Description (string): Sub-category description
        - CategoryId (Guid): Parent category ID
        - CategoryName (string): Parent category name
        - IsActive (bool): Whether sub-category is active
        - CreatedAt (DateTime): Creation timestamp
        - UpdatedAt (DateTime?): Last update timestamp
        - IsDeleted (bool): Soft delete flag

        EXAMPLES:
        - "active subcategories" → IsActive = true & IsDeleted = false
        - "subcategories for Hardware category" → CategoryName *= 'Hardware'
        - "subcategories with Printer in name" → Name *= 'Impressora'
        """;

    private static string DepartmentMetadata => """
        - Id (Guid): Unique identifier
        - Name (string): Department name
        - Description (string): Department description
        - ManagerId (Guid?): Department manager ID (nullable)
        - ManagerEmail (string): Manager email
        - IsActive (bool): Whether department is active
        - CreatedAt (DateTime): Creation timestamp
        - UpdatedAt (DateTime?): Last update timestamp
        - IsDeleted (bool): Soft delete flag

        EXAMPLES:
        - "active departments" → IsActive = true & IsDeleted = false
        - "IT department" → Name *= 'TI'
        - "departments with manager" → ManagerId != null
        """;

    private static string OrderTypeMetadata => """
        - Id (Guid): Unique identifier
        - Name (string): Order type name
        - Description (string): Order type description
        - Icon (string): Icon identifier
        - Color (string): Color code
        - DefaultPriority (int): Default priority for this type (1-4)
        - RequiresApproval (bool): Whether this type requires approval
        - SlaHours (int): Service Level Agreement in hours
        - IsActive (bool): Whether type is active
        - CreatedAt (DateTime): Creation timestamp
        - UpdatedAt (DateTime?): Last update timestamp
        - IsDeleted (bool): Soft delete flag

        EXAMPLES:
        - "types that require approval" → RequiresApproval = true
        - "active order types" → IsActive = true & IsDeleted = false
        - "types with SLA under 24 hours" → SlaHours < 24
        - "incident types" → Name *= 'Incidente'
        """;

    private static string StatusTypeMetadata => """
        - Id (Guid): Unique identifier
        - Name (string): Status type name
        - Description (string): Status type description
        - Icon (string): Icon identifier
        - Color (string): Color code
        - IsFinal (bool): Whether this is a final/closed status
        - Order (int): Display order
        - IsActive (bool): Whether status is active
        - CreatedAt (DateTime): Creation timestamp
        - UpdatedAt (DateTime?): Last update timestamp
        - IsDeleted (bool): Soft delete flag

        EXAMPLES:
        - "final statuses" → IsFinal = true
        - "active statuses" → IsActive = true & IsDeleted = false
        - "open status" → Name *= 'Aberto'
        - "statuses ordered by position" → ORDER: Order
        """;

    #endregion
}
