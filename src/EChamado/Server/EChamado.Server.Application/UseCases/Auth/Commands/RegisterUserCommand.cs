using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Domain.Domains.Identities;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Application.UseCases.Auth.Commands;

public class RegisterUserCommand : BrighterRequest<BaseResult<LoginResponseViewModel>>
{
    [Required(ErrorMessage = "O campo {0} é requerido")]
    [EmailAddress(ErrorMessage = "O campo {0} é inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é requeridod")]
    [StringLength(255, ErrorMessage = "O campo  {0} deve está entre {2} e {1} caracteres", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    public RegisterUserCommand()
    {
    }

    public RegisterUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public ApplicationUser ToDomain()
    {
        var entity = new ApplicationUser();

        entity.UserName = Email;
        entity.Email = Email;
        entity.EmailConfirmed = true;

        return entity;
    }
}
