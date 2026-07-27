using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Server.Application.UseCases.Categories.Queries;
using EChamado.Server.Endpoints.Categories.DTOs;

namespace EChamado.Server.Endpoints.Categories.DTOs;

/// <summary>
/// Extensões para mapeamento entre DTOs de Categories e comandos/queries
/// </summary>
public static class CategoriesDTOExtensions
{
    /// <summary>
    /// Converte CreateCategoryRequestDto para CreateCategoryCommand
    /// </summary>
    public static CreateCategoryCommand ToCommand(this CreateCategoryRequestDto requestDto)
    {
        return new CreateCategoryCommand
        {
            Name = requestDto.Name,
            Description = requestDto.Description ?? string.Empty
        };
    }

    /// <summary>
    /// Converte UpdateCategoryRequestDto para UpdateCategoryCommand
    /// </summary>
    public static UpdateCategoryCommand ToCommand(this UpdateCategoryRequestDto requestDto)
    {
        return new UpdateCategoryCommand
        {
            Id = Guid.Parse(requestDto.Id),
            Name = requestDto.Name,
            Description = requestDto.Description ?? string.Empty
        };
    }

    /// <summary>
    /// Converte SearchCategoriesParametersDto para SearchCategoriesQuery
    /// </summary>
    public static SearchCategoriesQuery ToQuery(this SearchCategoriesParametersDto parametersDto)
    {
        return new SearchCategoriesQuery
        {
            Name = parametersDto.Name ?? string.Empty,
            Description = parametersDto.Description ?? string.Empty,
            PageIndex = parametersDto.PageIndex,
            PageSize = parametersDto.PageSize
        };
    }
}