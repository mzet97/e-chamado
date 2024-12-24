using EChamado.Core.Responses;
using EChamado.Core.Services.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Application.Features.Roles.Commands.Handlers;

public class DeleteRoleCommandHandler(
    IRoleService roleService,
    ILogger<DeleteRoleCommandHandler> logger) :
    IRequestHandler<DeleteRoleCommand, BaseResult>
{
    public async Task<BaseResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

      
        var role = await roleService.GetRoleByIdAsync(request.Id);

        if(role == null)
            throw new Exception("Erro ao deletar");

        var result = await roleService.DeleteRoleAsync(request.Id);

        if (!result.Succeeded || result == null)
            throw new Exception("Erro ao deletar");

        logger.LogInformation("Role deletada com sucesso: ", role);

        return new BaseResult(true, "Deletada com sucesso");
    }
}
