using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public record DeleteCategoryCommand(Guid CategoryId) : IRequest<BaseResult>;
