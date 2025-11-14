using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Comments.Notifications;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Exceptions;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Comments.Commands;

public class CreateCommentCommandHandler(
    IUnitOfWork unitOfWork,
    IAmACommandProcessor commandProcessor,
    ILogger<CreateCommentCommandHandler> logger) :
    RequestHandlerAsync<CreateCommentCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<CreateCommentCommand> HandleAsync(CreateCommentCommand command, CancellationToken cancellationToken = default)
    {
        var order = await unitOfWork.Orders.GetByIdAsync(command.OrderId);

        if (order == null)
        {
            logger.LogError("Order {OrderId} not found", command.OrderId);
            throw new NotFoundException($"Order {command.OrderId} not found");
        }

        var entity = Comment.Create(
            command.Text,
            command.OrderId,
            command.UserId,
            command.UserEmail);

        if (!entity.IsValid())
        {
            logger.LogError("Validate Comment has error");
            throw new ValidationException("Validate Comment has error", entity.GetErrors());
        }

        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Comments.AddAsync(entity);

        await unitOfWork.CommitAsync();

        await commandProcessor.PublishAsync(new CreatedCommentNotification(
            entity.Id,
            entity.Text,
            entity.OrderId,
            entity.UserId,
            entity.UserEmail), cancellationToken: cancellationToken);

        logger.LogInformation("Comment {CommentId} created successfully for Order {OrderId}",
            entity.Id, command.OrderId);

        command.Result = new BaseResult<Guid>(entity.Id);
        return await base.HandleAsync(command, cancellationToken);
    }
}
