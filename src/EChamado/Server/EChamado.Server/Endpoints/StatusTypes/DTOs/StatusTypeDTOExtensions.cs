using EChamado.Server.Application.UseCases.StatusTypes.Commands;

namespace EChamado.Server.Endpoints.StatusTypes.DTOs;

public static class StatusTypeDTOExtensions
{
    public static CreateStatusTypeCommand ToCommand(this CreateStatusTypeRequest request)
    {
        return new CreateStatusTypeCommand(
            request.Name,
            request.Description ?? string.Empty
        );
    }

    public static UpdateStatusTypeCommand ToCommand(this UpdateStatusTypeRequest request)
    {
        return new UpdateStatusTypeCommand(
            request.Id,
            request.Name,
            request.Description ?? string.Empty
        );
    }

    public static DeletesStatusTypeCommand ToCommand(this DeleteStatusTypeRequest request)
    {
        return new DeletesStatusTypeCommand(new[] { request.Id });
    }
}
