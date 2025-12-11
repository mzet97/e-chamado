using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Comments.Queries;

public class SearchCommentsQuery : BrighterRequest<BaseResultList<CommentViewModel>>
{
    // BaseSearch properties
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? Order { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    // Specific properties
    public string Text { get; set; } = string.Empty;
    public Guid? OrderId { get; set; }
    public Guid? UserId { get; set; }
    public string UserEmail { get; set; } = string.Empty;
}
