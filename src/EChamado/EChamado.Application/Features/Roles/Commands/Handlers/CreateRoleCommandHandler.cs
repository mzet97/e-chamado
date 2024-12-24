using EChamado.Application.Features.Departments.Commands.Handlers;
using EChamado.Core.Domains.Identities;
using EChamado.Core.Responses;
using EChamado.Core.Services.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EChamado.Application.Features.Roles.Commands.Handlers;

public class CreateRoleCommandHandler(
    IRoleService roleService,
    ILogger<CreateRoleCommandHandler> logger) :
    IRequestHandler<CreateRoleCommand, BaseResult<Guid>>
{
    public async Task<BaseResult<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if(request == null)
            throw new ArgumentNullException(nameof(request));

        if(string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentNullException(nameof(request.Name));

        var result = await roleService.CreateRoleAsync(ApplicationRole.Create(request.Name));

        if(!result.Succeeded || result == null)
            throw new Exception("Erro ao criar");

        var role = await roleService.GetRoleByNameAsync(request.Name);

        if(role == null)
            throw new Exception("Erro ao criar");

        logger.LogInformation("Role criada com sucesso: ", role);

        return new BaseResult<Guid>(role.Id, true, "Criado com sucesso");
    }
}
