using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Auth.DTOs;

/// <summary>
/// DTO para requisição de login
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Email do usuário para autenticação
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário para autenticação
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string Password { get; set; } = string.Empty;
}
