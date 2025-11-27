using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Roles.Commands.Handlers;

public class CreateRoleCommandHandler(
    IRoleService roleService,
    ILogger<CreateRoleCommandHandler> logger) :
    RequestHandlerAsync<CreateRoleCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateRoleCommand> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        if (string.IsNullOrWhiteSpace(command.Name))
            throw new ArgumentNullException(nameof(command.Name));

        var result = await roleService.CreateRoleAsync(ApplicationRole.Create(command.Name));

        if (!result.Succeeded || result == null)
            throw new Exception("Erro ao criar");

        var role = await roleService.GetRoleByNameAsync(command.Name);

        if (role == null)
            throw new Exception("Erro ao criar");

        logger.LogInformation("Role criada com sucesso: {RoleId} - {RoleName}", role.Id, role.Name);

        command.Result = new BaseResult<Guid>(role.Id, true, "Criado com sucesso");
        return await base.HandleAsync(command, cancellationToken);
    }
}
