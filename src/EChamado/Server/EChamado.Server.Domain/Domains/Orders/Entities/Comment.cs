using EChamado.Server.Domain.Domains.Orders.Entities.Validations;
using EChamado.Server.Domain.Domains.Orders.Events.Comments;
using EChamado.Shared.Shared;

namespace EChamado.Server.Domain.Domains.Orders.Entities;

public class Comment : Entity
{
    public string Text { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public string UserEmail { get; private set; }

    // EF navigation property
    public Order Order { get; set; } = null!;

    public Comment(
        Guid id,
        string text,
        Guid orderId,
        Guid userId,
        string userEmail,
        DateTime createdAt,
        DateTime? updatedAt,
        DateTime? deletedAt,
        bool isDeleted) : base(id, createdAt, updatedAt, deletedAt, isDeleted)
    {
        Text = text;
        OrderId = orderId;
        UserId = userId;
        UserEmail = userEmail;
        Validate();
    }

    public override void Validate()
    {
        var validator = new CommentValidation();
        var result = validator.Validate(this);
        if (!result.IsValid)
        {
            _errors = result.Errors.Select(x => x.ErrorMessage);
            _isValid = false;
        }
        else
        {
            _errors = Enumerable.Empty<string>();
            _isValid = true;
        }
    }

    public static Comment Create(
        string text,
        Guid orderId,
        Guid userId,
        string userEmail)
    {
        var comment = new Comment(
            Guid.NewGuid(),
            text,
            orderId,
            userId,
            userEmail,
            DateTime.Now,
            null,
            null,
            false);

        comment.AddEvent(new CommentCreated(comment));

        return comment;
    }
}
