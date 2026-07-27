using EChamado.Server.Application.UseCases.OrderTypes.Commands;

namespace EChamado.Server.Endpoints.OrderTypes.DTOs;

public static class OrderTypeDTOExtensions
{
    public static CreateOrderTypeCommand ToCommand(this CreateOrderTypeRequest request)
    {
        return new CreateOrderTypeCommand(
            request.Name,
            request.Description ?? string.Empty
        );
    }

    public static UpdateOrderTypeCommand ToCommand(this UpdateOrderTypeRequest request)
    {
        return new UpdateOrderTypeCommand(
            request.Id,
            request.Name,
            request.Description ?? string.Empty
        );
    }

    public static DeleteOrderTypeCommand ToCommand(this DeleteOrderTypeRequest request)
    {
        return new DeleteOrderTypeCommand(request.Id);
    }
}
