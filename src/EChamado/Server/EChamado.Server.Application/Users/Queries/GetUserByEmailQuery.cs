using EChamado.Server.Application.Common.Messaging;

namespace EChamado.Server.Application.Users.Queries;

public sealed class GetUserByEmailQuery : BrighterRequest<UserDetailsDto?>
{
    public string Email { get; init; }

    public GetUserByEmailQuery(string email)
    {
        Email = email;
    }
}
