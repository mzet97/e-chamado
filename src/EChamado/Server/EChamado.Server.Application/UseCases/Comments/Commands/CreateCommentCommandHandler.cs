using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;
using EChamado.Server.Application.UseCases.Comments.Notifications;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public class CreateCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    ILogger<CreateCommentCommandHandler> logger) :
    IRequestHandler<CreateCommentCommand, BaseResult<Guid>>
{
    public async Task<BaseResult<Guid>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", request.OrderId);
            throw new NotFoundException($"Order {request.OrderId} not found");
        }

        var entity = Comment.Create(
            request.Text,
            request.OrderId,
            request.UserId,
            request.UserEmail);

        if (!entity.IsValid())
        {
            logger.LogError("Validate Comment has error");
            throw new ValidationException("Validate Comment has error", entity.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Comments.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await mediator.Publish(new CreatedCommentNotification(
            entity.Id,
            entity.Text,
            entity.OrderId,
            entity.UserId,
            entity.UserEmail), cancellationToken);

        logger.LogInformation("Comment {CommentId} created successfully for Order {OrderId}",
            entity.Id, request.OrderId);

        return new BaseResult<Guid>(entity.Id);
    }
}
