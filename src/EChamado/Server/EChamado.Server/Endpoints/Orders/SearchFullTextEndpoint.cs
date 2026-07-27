using EChamado.Server.Common.Api;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Server.Infrastructure.Persistence;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Endpoints.Orders;

/// <summary>
/// Endpoint de busca full-text nos chamados usando PostgreSQL tsvector.
/// Usa raw SQL para a query tsvector (não suportado nativamente pelo EF Core).
/// </summary>
public sealed class SearchFullTextEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/search", HandleAsync)
            .WithName("Buscar chamados por texto completo")
            .WithTags("Order")
            .WithDescription("Busca full-text em título e descrição dos chamados usando PostgreSQL tsvector com fallback ILIKE")
            .Produces<BaseResultList<SearchFullTextResult>>()
            .Produces(StatusCodes.Status400BadRequest);

    private static async Task<IResult> HandleAsync(
        [FromServices] ApplicationDbContext dbContext,
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return TypedResults.BadRequest(new BaseResultList<SearchFullTextResult>(
                Enumerable.Empty<SearchFullTextResult>(),
                new Shared.Responses.PagedResult(),
                success: false,
                message: "Parâmetro 'q' é obrigatório"));
        }

        var searchTerm = q.Trim();
        var skip = (page - 1) * pageSize;

        // Busca full-text usando tsvector (português)
        var countSql = @"
            SELECT COUNT(*) FROM ""Order""
            WHERE ""IsDeleted"" = false
              AND ""SearchVector"" @@ to_tsquery('portuguese', {0})";

        var searchSql = @"
            SELECT ""Id"", ""Title"", ""Description"", ""OpeningDate"",
                   ""StatusId"", ""CategoryId"", ""DepartmentId""
            FROM ""Order""
            WHERE ""IsDeleted"" = false
              AND ""SearchVector"" @@ to_tsquery('portuguese', {0})
            ORDER BY ""OpeningDate"" DESC
            LIMIT {1} OFFSET {2}";

        // Converte termo de busca para formato tsquery (ex: "computador liga" → "computador & liga")
        var tsQuery = string.Join(" & ", searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries));

        int totalCount;
        List<SearchFullTextResult> results;

        try
        {
            totalCount = await dbContext.Database
                .SqlQueryRaw<int>(countSql, tsQuery)
                .FirstOrDefaultAsync(cancellationToken);

            results = await dbContext.Database
                .SqlQueryRaw<SearchFullTextResult>(searchSql, tsQuery, pageSize, skip)
                .ToListAsync(cancellationToken);
        }
        catch
        {
            // Fallback: busca simples com ILIKE se tsvector falhar
            var fallbackCountSql = @"
                SELECT COUNT(*) FROM ""Order""
                WHERE ""IsDeleted"" = false
                  AND (""Title"" ILIKE {0} OR ""Description"" ILIKE {0})";

            var fallbackSearchSql = @"
                SELECT ""Id"", ""Title"", ""Description"", ""OpeningDate"",
                       ""StatusId"", ""CategoryId"", ""DepartmentId""
                FROM ""Order""
                WHERE ""IsDeleted"" = false
                  AND (""Title"" ILIKE {0} OR ""Description"" ILIKE {0})
                ORDER BY ""OpeningDate"" DESC
                LIMIT {1} OFFSET {2}";

            var likePattern = $"%{searchTerm}%";

            totalCount = await dbContext.Database
                .SqlQueryRaw<int>(fallbackCountSql, likePattern)
                .FirstOrDefaultAsync(cancellationToken);

            results = await dbContext.Database
                .SqlQueryRaw<SearchFullTextResult>(fallbackSearchSql, likePattern, pageSize, skip)
                .ToListAsync(cancellationToken);
        }

        var paged = Shared.Responses.PagedResult.Create(page, pageSize, totalCount);
        return TypedResults.Ok(new BaseResultList<SearchFullTextResult>(results, paged));
    }
}

public sealed class SearchFullTextResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? OpeningDate { get; set; }
    public Guid StatusId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid DepartmentId { get; set; }
}
