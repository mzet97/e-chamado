using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.Comments;
using EChamado.Shared.Domain;
using EChamado.Shared.Services;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class Comment : SoftDeletableEntity<Comment>
{
    public string Text { get; private set; } = string.Empty;
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserEmail { get; private set; } = string.Empty;
    public bool IsInternal { get; private set; }

    public Order Order { get; private set; } = null!;

    private Comment() : base(new CommentValidation()) { }

    private Comment(
        Guid id,
        string text,
        Guid orderId,
        Guid userId,
        string userEmail,
        bool isInternal,
        IDateTimeProvider dateTimeProvider)
        : base(new CommentValidation())
    {
        Id = id;

        Text = text;
        OrderId = orderId;
        UserId = userId;
        UserEmail = userEmail;
        IsInternal = isInternal;

        MarkCreated(dateTimeProvider.UtcNow);
        Validate();
    }

    public static Comment Create(
        string text,
        Guid orderId,
        Guid userId,
        string userEmail,
        IDateTimeProvider dateTimeProvider,
        bool isInternal = false)
    {
        var comment = new Comment(
            Guid.NewGuid(),
            text,
            orderId,
            userId,
            userEmail,
            isInternal,
            dateTimeProvider);

        comment.AddEvent(new CommentCreated(
            comment.Id,
            comment.OrderId,
            comment.UserId,
            comment.UserEmail,
            comment.Text
        ));
        return comment;
    }

    public override void Validate()
    {
        var validator = new CommentValidation();
        var result = validator.Validate(this);

        _errors = result.Errors.Select(x => x.ErrorMessage);
        _isValid = result.IsValid;
    }
}
