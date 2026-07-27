using EChamado.Server.Application.UseCases.OrderTypes.Commands;
using EChamado.Server.Application.UseCases.OrderTypes.Queries;
using EChamado.Server.Endpoints.OrderTypes.DTOs;

namespace EChamado.Server.Endpoints.OrderTypes.Extensions;

/// <summary>
/// Extensões para mapeamento entre DTOs e comandos/queries do OrderTypes
/// </summary>
public static class OrderTypesDTOExtensions
{
    /// <summary>
    /// Converte CreateOrderTypeRequestDto para CreateOrderTypeCommand
    /// </summary>
    public static CreateOrderTypeCommand ToCommand(this CreateOrderTypeRequestDto requestDto)
    {
        return new CreateOrderTypeCommand(requestDto.Name, requestDto.Description ?? string.Empty);
    }

    /// <summary>
    /// Converte UpdateOrderTypeRequestDto para UpdateOrderTypeCommand
    /// </summary>
    public static UpdateOrderTypeCommand ToCommand(this UpdateOrderTypeRequestDto requestDto, Guid id)
    {
        return new UpdateOrderTypeCommand(id, requestDto.Name, requestDto.Description ?? string.Empty);
    }

    /// <summary>
    /// Converte SearchOrderTypesParametersDto para SearchOrderTypesQuery
    /// </summary>
    public static SearchOrderTypesQuery ToQuery(this SearchOrderTypesParametersDto parameters)
    {
        return new SearchOrderTypesQuery
        {
            Name = parameters.Name ?? string.Empty,
            Description = parameters.Description ?? string.Empty,
            CreatedAt = DateTime.MinValue,
            UpdatedAt = DateTime.MinValue,
            DeletedAt = null,
            Order = string.Empty,
            PageIndex = parameters.PageIndex,
            PageSize = parameters.PageSize
        };
    }
}