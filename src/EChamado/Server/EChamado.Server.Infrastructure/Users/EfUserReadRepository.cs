using EChamado.Server.Application.Users;
using EChamado.Server.Application.Users.Abstractions;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EChamado.Server.Infrastructure.Users;

public sealed class EfUserReadRepository(ApplicationDbContext dbContext)
    : IUserReadRepository
{
    public async Task<UserDetailsDto?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Email == email)
            .Select(u => new UserDetailsDto(
                u.Id,
                u.Email ?? string.Empty,
                string.IsNullOrWhiteSpace(u.FullName) ? (u.UserName ?? string.Empty) : u.FullName,
                u.CreatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<UserListItemDto>> SearchAsync(
        UserSearchFilter filter,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.EmailContains))
        {
            var term = $"%{filter.EmailContains.Trim().ToLowerInvariant()}%";
            query = query.Where(u =>
                EF.Functions.Like(u.Email.ToLower(), term));
        }

        if (!string.IsNullOrWhiteSpace(filter.NameContains))
        {
            var term = $"%{filter.NameContains.Trim().ToLowerInvariant()}%";
            query = query.Where(u =>
                EF.Functions.Like(((u.FullName ?? u.UserName) ?? string.Empty).ToLower(), term));
        }

        query = ApplyOrdering(query, filter);

        var total = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(u => new UserListItemDto(
                u.Id,
                u.Email ?? string.Empty,
                string.IsNullOrWhiteSpace(u.FullName) ? (u.UserName ?? string.Empty) : u.FullName,
                u.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<UserListItemDto>(items, filter.Page, filter.PageSize, total);
    }

    private static IQueryable<ApplicationUser> ApplyOrdering(
        IQueryable<ApplicationUser> source,
        UserSearchFilter filter)
    {
        return (filter.SortBy?.ToLowerInvariant()) switch
        {
            "email" => filter.SortDescending
                ? source.OrderByDescending(u => u.Email)
                : source.OrderBy(u => u.Email),
            "createdat" or "createdatutc" => filter.SortDescending
                ? source.OrderByDescending(u => u.CreatedAtUtc)
                : source.OrderBy(u => u.CreatedAtUtc),
            "name" or null or "" => filter.SortDescending
                ? source.OrderByDescending(u => u.FullName ?? u.UserName ?? string.Empty)
                : source.OrderBy(u => u.FullName ?? u.UserName ?? string.Empty),
            _ => source.OrderBy(u => u.FullName ?? u.UserName ?? string.Empty)
        };
    }
}
