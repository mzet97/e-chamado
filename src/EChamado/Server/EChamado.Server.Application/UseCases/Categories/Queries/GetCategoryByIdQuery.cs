using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

public record GetCategoryByIdQuery(Guid CategoryId) : IRequest<BaseResult<CategoryViewModel>>;
