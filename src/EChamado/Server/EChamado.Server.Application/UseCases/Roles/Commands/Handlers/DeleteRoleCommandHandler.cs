using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Roles.Commands.Handlers;

public class DeleteRoleCommandHandler(
    IRoleService roleService,
    ILogger<DeleteRoleCommandHandler> logger) :
    RequestHandlerAsync<DeleteRoleCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<DeleteRoleCommand> HandleAsync(DeleteRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));


        var role = await roleService.GetRoleByIdAsync(command.Id);

        if(role == null)
            throw new Exception("Erro ao deletar");

        var result = await roleService.DeleteRoleAsync(command.Id);

        if (!result.Succeeded || result == null)
            throw new Exception("Erro ao deletar");

        logger.LogInformation("Role deletada com sucesso: ", role);

        command.Result = new BaseResult(true, "Deletada com sucesso");
        return await base.HandleAsync(command, cancellationToken);
    }
}
