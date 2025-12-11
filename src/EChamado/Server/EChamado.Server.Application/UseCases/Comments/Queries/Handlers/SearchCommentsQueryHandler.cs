using EChamado.Server.Application.UseCases.Comments.ViewModels;
using EChamado.Server.Domain.Domains.Orders.Entities;
using EChamado.Server.Domain.Repositories;
using EChamado.Shared.Responses;
using LinqKit;
using Paramore.Brighter;
using System.Linq.Expressions;

namespace EChamado.Server.Application.UseCases.Comments.Queries.Handlers;

public class SearchCommentsQueryHandler(IUnitOfWork unitOfWork) :
    RequestHandlerAsync<SearchCommentsQuery>
{
    public override async Task<SearchCommentsQuery> HandleAsync(
        SearchCommentsQuery query,
        CancellationToken cancellationToken = default)
    {
        Expression<Func<Comment, bool>>? filter = PredicateBuilder.New<Comment>(true);
        Func<IQueryable<Comment>, IOrderedQueryable<Comment>>? orderBy = null;

        if (!string.IsNullOrWhiteSpace(query.Text))
        {
            filter = filter.And(x => x.Text == query.Text);
        }

        if (query.OrderId.HasValue && query.OrderId.Value != Guid.Empty)
        {
            filter = filter.And(x => x.OrderId == query.OrderId.Value);
        }

        if (query.UserId.HasValue && query.UserId.Value != Guid.Empty)
        {
            filter = filter.And(x => x.UserId == query.UserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.UserEmail))
        {
            filter = filter.And(x => x.UserEmail == query.UserEmail);
        }

        if (query.CreatedAt != default)
        {
            filter = filter.And(x => x.CreatedAtUtc == query.CreatedAt);
        }

        if (query.UpdatedAt != default)
        {
            filter = filter.And(x => x.UpdatedAtUtc == query.UpdatedAt);
        }

        if (query.DeletedAt != new DateTime())
        {
            filter = filter.And(x => x.DeletedAtUtc == query.DeletedAt);
        }

        if (!string.IsNullOrWhiteSpace(query.Order))
        {
            switch (query.Order)
            {
                case "Id":
                    orderBy = x => x.OrderBy(n => n.Id);
                    break;

                case "Text":
                    orderBy = x => x.OrderBy(n => n.Text);
                    break;

                case "OrderId":
                    orderBy = x => x.OrderBy(n => n.OrderId);
                    break;

                case "UserId":
                    orderBy = x => x.OrderBy(n => n.UserId);
                    break;

                case "UserEmail":
                    orderBy = x => x.OrderBy(n => n.UserEmail);
                    break;

                case "CreatedAt":
                    orderBy = x => x.OrderBy(n => n.CreatedAtUtc);
                    break;

                case "UpdatedAt":
                    orderBy = x => x.OrderBy(n => n.UpdatedAtUtc);
                    break;

                case "DeletedAt":
                    orderBy = x => x.OrderBy(n => n.DeletedAtUtc);
                    break;

                default:
                    orderBy = x => x.OrderBy(n => n.Id);
                    break;
            }
        }

        var result = await unitOfWork.Comments
            .SearchAsync(
                filter,
                orderBy,
                query.PageSize,
                query.PageIndex);

        var items = result.Data.Select(c => new CommentViewModel(
            c.Id,
            c.Text,
            c.OrderId,
            c.UserId,
            c.UserEmail,
            c.CreatedAtUtc,
            c.UpdatedAtUtc,
            c.DeletedAtUtc,
            c.IsDeleted
        )).ToList();

        query.Result = new BaseResultList<CommentViewModel>(items, result.PagedResult);

        return await base.HandleAsync(query, cancellationToken);
    }
}
