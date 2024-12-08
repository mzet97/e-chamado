using EChamado.Application.Features.Auth.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Auth.Commands;

public class GetTokenCommand : 
    IRequest<BaseResult<LoginResponseViewModel>>
{
    public string Email { get; set; } = string.Empty;
}