using System.ComponentModel.DataAnnotations;

namespace EChamado.Server.Endpoints.Auth.DTOs;

/// <summary>
/// DTO para requisição de registro de usuário
/// </summary>
public class RegisterRequestDto
{
    /// <summary>
    /// Email do usuário para cadastro
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} deve conter um email válido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário para cadastro
    /// </summary>
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(255, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
