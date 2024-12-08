using EChamado.Application.Features.Auth.Notifications;
using EChamado.Application.Features.Auth.ViewModels;
using EChamado.Core.Responses;
using EChamado.Core.Services.Interface;
using MediatR;
using System.Text;

namespace EChamado.Application.Features.Auth.Commands.Handlers;

public class RegisterUserCommandHandler(
    IApplicationUserService applicationUserService,
    IMediator mediator
    ) : IRequestHandler<RegisterUserCommand, BaseResult<LoginResponseViewModel>>
{
    public async Task<BaseResult<LoginResponseViewModel>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = request.ToDomain();

        var resultCreateUser = await applicationUserService.CreateAsync(user, request.Password);

        if (resultCreateUser.Succeeded)
        {
            await applicationUserService.TrySignInAsync(user);
            return await mediator.Send(new GetTokenCommand { Email = request.Email });
        }

        var sb = new StringBuilder();
        foreach (var error in resultCreateUser.Errors)
        {
            sb.Append(error.Description);
        }

        await mediator.Publish(new RegisterUserNotification { Email = request.Email, Message = sb.ToString() });

        return await mediator.Send(new GetTokenCommand { Email = request.Email });
    }
}
