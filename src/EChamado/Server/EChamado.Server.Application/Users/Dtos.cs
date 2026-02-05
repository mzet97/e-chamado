namespace EChamado.Server.Application.Users;

public sealed record UserDetailsDto(
    Guid Id,
    string Email,
    string FullName,
    DateTime CreatedAtUtc);

public sealed record UserListItemDto(
    Guid Id,
    string Email,
    string FullName,
    DateTime CreatedAtUtc);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    long TotalCount)
{
    public int TotalPages => PageSize <= 0
        ? 0
        : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
