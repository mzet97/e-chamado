namespace EChamado.Server.Domain.Domains.Users;

public sealed class User
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}
