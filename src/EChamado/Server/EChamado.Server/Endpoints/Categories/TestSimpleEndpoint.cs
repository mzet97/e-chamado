using EChamado.Server.Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Categories;

public class TestSimpleEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/test-simple", HandleAsync)
            .WithName("Teste simples")
            .Produces<string>();

    private static async Task<IResult> HandleAsync()
    {
        return TypedResults.Ok(new { message = "API Server funcionando!", timestamp = DateTime.Now });
    }
}