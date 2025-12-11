namespace EChamado.Client.Models;

public record UserResponse(
    Guid Id,
    string Email,
    string UserName,
    string? FirstName,
    string? LastName,
    bool EmailConfirmed,
    bool PhoneNumberConfirmed,
    bool TwoFactorEnabled,
    DateTimeOffset? LockoutEnd,
    bool LockoutEnabled,
    int AccessFailedCount,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<string> Roles
);

public record CreateUserRequest(string Email, string UserName, string Password, string FirstName, string LastName);
public record UpdateUserRequest(string UserName, string FirstName, string LastName);
public record ChangePasswordRequest(string OldPassword, string NewPassword);