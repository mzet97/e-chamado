using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public record CreateCategoryCommand(
    string Name,
    string Description
) : IRequest<BaseResult<Guid>>;
