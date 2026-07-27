namespace EChamado.Server.Application.Users;

public sealed class UserSearchFilter
{
    public string? EmailContains { get; init; }
    public string? NameContains { get; init; }
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;

    public void Normalize()
    {
        if (Page < 1)
        {
            Page = 1;
        }

        if (PageSize < 1)
        {
            PageSize = 25;
        }
        else if (PageSize > 200)
        {
            PageSize = 200;
        }
    }
}
