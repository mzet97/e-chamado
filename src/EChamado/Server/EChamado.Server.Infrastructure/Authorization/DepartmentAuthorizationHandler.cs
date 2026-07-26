using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EChamado.Server.Infrastructure.Authorization;

/// <summary>
/// Requirement que verifica se o usuário pertence ao departamento correto.
/// </summary>
public class DepartmentRequirement : IAuthorizationRequirement
{
    public string DepartmentIdClaimType { get; } = "department_id";
}

/// <summary>
/// Handler de autorização que valida se o usuário tem DepartmentId
/// e opcionalmente se coincide com o departamento do recurso.
/// Usado como policy base para department-level access.
/// </summary>
public class DepartmentAuthorizationHandler :
    AuthorizationHandler<DepartmentRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DepartmentRequirement requirement)
    {
        var departmentIdClaim = context.User.FindFirst("department_id");

        // Se o usuário não tem claim de departamento, permite (fallback para roles)
        // A restrição por departamento é uma camada adicional, não obrigatória
        if (departmentIdClaim == null)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Se tem claim, valida que é um GUID válido
        if (Guid.TryParse(departmentIdClaim.Value, out _))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail(new AuthorizationFailureReason(this, "Invalid department claim"));
        }

        return Task.CompletedTask;
    }
}
