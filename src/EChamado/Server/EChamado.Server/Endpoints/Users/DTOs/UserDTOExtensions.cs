using EChamado.Server.Application.UseCases.Users.Queries;
using EChamado.Server.Application.UseCases.Users.ViewModels;

namespace EChamado.Server.Endpoints.Users.DTOs;

public static class UserDTOExtensions
{
    public static GetAllUsersQuery ToQuery(this GetAllUsersRequest request)
    {
        return new GetAllUsersQuery();
    }
}
