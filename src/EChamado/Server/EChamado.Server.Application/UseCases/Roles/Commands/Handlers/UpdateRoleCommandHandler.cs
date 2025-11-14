using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Roles.Commands.Handlers;

public class UpdateRoleCommandHandler(
    IRoleService roleService,
    ILogger<UpdateRoleCommandHandler> logger) :
    RequestHandlerAsync<UpdateRoleCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<UpdateRoleCommand> HandleAsync(UpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentNullException(nameof(command.Name));

        var role = await roleService.GetRoleByIdAsync(command.Id);

        var inRoleNameDb = await roleService.GetRoleByNameAsync(command.Name);

        if (role == null || inRoleNameDb != null)
            throw new Exception("Erro ao atualizar");

        role.Name = command.Name;

        var result = await roleService.UpdateRoleAsync(role);

        if (!result.Succeeded || result == null)
            throw new Exception("Erro ao atualizar");

        logger.LogInformation("Role atualizar com sucesso: ", role);

        command.Result = new BaseResult(true, "Atualizada com sucesso");
        return await base.HandleAsync(command, cancellationToken);
    }
}
