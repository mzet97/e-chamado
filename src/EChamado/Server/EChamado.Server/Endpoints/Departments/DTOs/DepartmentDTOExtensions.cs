using EChamado.Server.Application.UseCases.Departments.Commands;

namespace EChamado.Server.Endpoints.Departments.DTOs;

public static class DepartmentDTOExtensions
{
    public static CreateDepartmentCommand ToCommand(this CreateDepartmentRequest request)
    {
        return new CreateDepartmentCommand(
            request.Name,
            request.Description ?? string.Empty
        );
    }

    public static UpdateDepartmentCommand ToCommand(this UpdateDepartmentRequest request)
    {
        return new UpdateDepartmentCommand(
            request.Id,
            request.Name,
            request.Description ?? string.Empty
        );
    }

    public static DeletesDepartmentCommand ToCommand(this DeleteDepartmentRequest request)
    {
        return new DeletesDepartmentCommand(request.Ids);
    }
}
