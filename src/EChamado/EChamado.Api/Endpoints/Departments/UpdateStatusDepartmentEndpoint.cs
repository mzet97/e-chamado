﻿using EChamado.Api.Common.Api;
using EChamado.Application.Features.Departments.Commands;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Api.Endpoints.Departments;

public class UpdateStatusDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPatch("/update-status", HandleAsync)
        .WithName("Desativa uma lista de departamentos")
        .WithSummary("Desativa uma lista de departamentos")
        .WithDescription("Desativa uma lista de departamentos")
        .WithOrder(8)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [FromBody] UpdateStatusDepartmentCommand command)
    {
        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}