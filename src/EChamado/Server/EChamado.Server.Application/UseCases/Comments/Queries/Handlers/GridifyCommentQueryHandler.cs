using EChamado.Server.Application.Common;
using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Server.Domain.Repositories.Orders;
using EChamado.Shared.Responses;
using Paramore.Darker;

namespace EChamado.Server.Application.UseCases.Comments.Queries.Handlers;

/// <summary>
/// Handler para processar queries de comments com Gridify
/// </summary>
public class GridifyCommentQueryHandler : QueryHandlerAsync<GridifyCommentQuery, BaseResultList<CommentViewModel>>
{
    private readonly ICommentRepository _commentRepository;

    public GridifyCommentQueryHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public override async Task<BaseResultList<CommentViewModel>> ExecuteAsync(GridifyCommentQuery request, CancellationToken cancellationToken = default)
    {
        // 1. Obtém a query base
        var query = _commentRepository.GetAllQueryable()
            .Where(c => !c.IsDeleted); // Filtro padrão: não retornar registros deletados

        // 2. Aplica Gridify (filtros, ordenação e paginação)
        var result = await query.ApplyGridifyAsync(request, cancellationToken);

        // 3. Mapeia as entidades para ViewModels
        var viewModels = result.Data.Select(comment => new CommentViewModel(
            comment.Id,
            comment.Text,
            comment.OrderId,
            comment.UserId,
            comment.UserEmail,
            comment.CreatedAtUtc,
            comment.UpdatedAtUtc,
            comment.DeletedAtUtc,
            comment.IsDeleted
        )).ToList();

        // 4. Retorna resultado paginado com ViewModels
        return new BaseResultList<CommentViewModel>(viewModels, result.PagedResult);
    }
}
