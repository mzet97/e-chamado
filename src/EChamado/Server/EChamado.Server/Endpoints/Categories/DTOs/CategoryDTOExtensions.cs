using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Server.Application.UseCases.Categories.Queries;

namespace EChamado.Server.Endpoints.Categories.DTOs;

public static class CategoryDTOExtensions
{
    public static CreateCategoryCommand ToCommand(this CreateCategoryRequest request)
    {
        return new CreateCategoryCommand(
            request.Name,
            request.Description ?? string.Empty
        );
    }

    public static UpdateCategoryCommand ToCommand(this UpdateCategoryRequest request)
    {
        return new UpdateCategoryCommand(
            request.Id,
            request.Name,
            request.Description ?? string.Empty
        );
    }

    public static DeleteCategoryCommand ToCommand(this DeleteCategoryRequest request)
    {
        return new DeleteCategoryCommand(request.Id);
    }
}
