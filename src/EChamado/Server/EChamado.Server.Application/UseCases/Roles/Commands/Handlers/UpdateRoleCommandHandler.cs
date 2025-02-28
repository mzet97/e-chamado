using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Server.Application.UseCases.Roles.Commands.Handlers;

public class UpdateRoleCommandHandler(
    IRoleService roleService,
    ILogger<UpdateRoleCommandHandler> logger) :
    IRequestHandler<UpdateRoleCommand, BaseResult>
{
    public async Task<BaseResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentNullException(nameof(request.Name));

        var role = await roleService.GetRoleByIdAsync(request.Id);

        var inRoleNameDb = await roleService.GetRoleByNameAsync(request.Name);

        if (role == null || inRoleNameDb != null)
            throw new Exception("Erro ao atualizar");

        role.Name = request.Name;

        var result = await roleService.UpdateRoleAsync(role);

        if (!result.Succeeded || result == null)
            throw new Exception("Erro ao atualizar");

        logger.LogInformation("Role atualizar com sucesso: ", role);

        return new BaseResult(true, "Atualizada com sucesso");
    }
}
