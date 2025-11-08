using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description
) : IRequest<BaseResult>;
