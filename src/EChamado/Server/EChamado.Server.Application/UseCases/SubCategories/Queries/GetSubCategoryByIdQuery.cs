using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.SubCategories.Queries;

public record GetSubCategoryByIdQuery(Guid SubCategoryId) : IRequest<BaseResult<SubCategoryViewModel>>;
