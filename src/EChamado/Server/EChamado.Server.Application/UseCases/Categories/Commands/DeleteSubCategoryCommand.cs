using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Categories.Commands;

public record DeleteSubCategoryCommand(Guid SubCategoryId) : IRequest<BaseResult>;
