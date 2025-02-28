using EChamado.Client.Application.UseCases.Auth.InputModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;

namespace EChamado.Client.Application.UseCases.Auth.Handlers.Interfaces;

public interface ILoginHandler
{
    Task<BaseResult<LoginResponseViewModel>> ExecuteAsync(LoginInputModel inputModel);
}
