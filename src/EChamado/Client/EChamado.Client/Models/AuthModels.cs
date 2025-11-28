using System.ComponentModel.DataAnnotations;

namespace EChamado.Client.Models;

public class LoginModel
{
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterModel
{
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "A confirmação de senha é obrigatória")]
    [Compare(nameof(Password), ErrorMessage = "As senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class AuthResult
{
    public bool Successful { get; set; }
    public string? Error { get; set; }
}
