using EChamado.Client.Application.UseCases.Auth.Handlers.Interfaces;
using EChamado.Client.Application.UseCases.Auth.InputModels;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using System.Net.Http.Json;

namespace EChamado.Client.Application.UseCases.Auth.Handlers;

public class RegisterHandler(IHttpClientFactory httpClientFactory) 
    : IRegisterHandler
{
    private readonly HttpClient _client = httpClientFactory
        .CreateClient("EChamado");

public async Task<BaseResult<LoginResponseViewModel>> ExecuteAsync(LoginInputModel inputModel)
{
    var result = await _client
       .PostAsJsonAsync("v1/auth/register", inputModel);

    if (result.IsSuccessStatusCode)
    {
        var data = await result
            .Content
            .ReadFromJsonAsync<BaseResult<LoginResponseViewModel>>();

        if (data != null && data.Success)
            return data;
    }

    return new BaseResult<LoginResponseViewModel>(null, success: false, message: "Erro durante o registro");
}
}
