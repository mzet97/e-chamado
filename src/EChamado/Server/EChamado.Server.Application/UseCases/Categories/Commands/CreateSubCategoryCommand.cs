using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public record CreateSubCategoryCommand(
    string Name,
    string Description,
    Guid CategoryId
) : IRequest<BaseResult<Guid>>;
