using EChamado.Application.Features.Auth.ViewModels;
using EChamado.Core.Domains.Identities;
using EChamado.Core.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace EChamado.Application.Features.Auth.Commands
{
    public class RegisterUserCommand : IRequest<BaseResult<LoginResponseViewModel>>
    {
        [Required(ErrorMessage = "O campo {0} é requerido")]
        [EmailAddress(ErrorMessage = "O campo {0} é inválido")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "O campo {0} é requeridod")]
        [StringLength(255, ErrorMessage = "O campo  {0} deve está entre {2} e {1} caracteres", MinimumLength = 6)]
        public required string Password { get; set; }


        public ApplicationUser ToDomain()
        {
            var entity = new ApplicationUser();

            entity.UserName = Email;
            entity.Email = Email;
            entity.EmailConfirmed = true;

            return entity;
        }
    }
}
