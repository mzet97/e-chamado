using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Server.Application.UseCases.Categories.Queries;
using EChamado.Server.Application.UseCases.SubCategories.Queries;

namespace EChamado.Server.Endpoints.SubCategories.DTOs;

public static class SubCategoryDTOExtensions
{
    public static CreateSubCategoryCommand ToCommand(this CreateSubCategoryRequest request)
    {
        return new CreateSubCategoryCommand(
            request.Name,
            request.Description ?? string.Empty,
            request.CategoryId
        );
    }

    public static UpdateSubCategoryCommand ToCommand(this UpdateSubCategoryRequest request)
    {
        return new UpdateSubCategoryCommand(
            request.Id,
            request.Name,
            request.Description ?? string.Empty,
            request.CategoryId
        );
    }

    public static DeleteSubCategoryCommand ToCommand(this DeleteSubCategoryRequest request)
    {
        return new DeleteSubCategoryCommand(request.Id);
    }

    public static GetSubCategoryByIdQuery ToQuery(this GetSubCategoryByIdRequest request)
    {
        return new GetSubCategoryByIdQuery(request.Id);
    }

    public static SearchSubCategoriesQuery ToQuery(this SearchSubCategoriesRequest request)
    {
        return new SearchSubCategoriesQuery
        {
            Name = request.Name ?? string.Empty,
            Description = string.Empty,
            CategoryId = request.CategoryId,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }
}
