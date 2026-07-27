using EChamado.Server.Application.Users;
using EChamado.Server.Application.Users.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;
using EChamado.Server.Application.Common.Messaging;

namespace EChamado.Server.Presentation.Api.Endpoints;

public static class UserReadEndpoints
{
    public static IEndpointRouteBuilder MapUserReadEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users").WithTags("Users (Read)");

        group.MapGet("/{email}", GetUserByEmailAsync)
            .WithSummary("Busca usuário pelo e-mail")
            .WithDescription("Retorna os detalhes do usuário a partir do e-mail informado.");

        group.MapGet("/", SearchUsersAsync)
            .WithSummary("Pesquisa usuários")
            .WithDescription("Retorna usuários paginados com filtros e ordenação.");

        return app;
    }

    private static async Task<Results<Ok<UserDetailsDto>, NotFound>> GetUserByEmailAsync(
        [FromRoute] string email,
        [FromServices] IAmACommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByEmailQuery(email);
        await commandProcessor.SendAsync(query, cancellationToken: cancellationToken);
        var dto = query.Result;

        return dto is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(dto);
    }

    private static async Task<Ok<PagedResult<UserListItemDto>>> SearchUsersAsync(
        [FromQuery] string? email,
        [FromQuery] string? name,
        [FromQuery] string? sortBy,
        [FromQuery] bool? desc,
        [FromQuery] int? page,
        [FromQuery(Name = "pageSize")] int? pageSize,
        [FromServices] IAmACommandProcessor commandProcessor,
        CancellationToken cancellationToken)
    {
        var filter = new UserSearchFilter
        {
            EmailContains = email,
            NameContains = name,
            SortBy = sortBy,
            SortDescending = desc ?? false,
            Page = page ?? 1,
            PageSize = pageSize ?? 25
        };

        var query = new SearchUsersQuery(filter);
        await commandProcessor.SendAsync(query, cancellationToken: cancellationToken);
        var result = query.Result;

        return TypedResults.Ok(result);
    }
}
