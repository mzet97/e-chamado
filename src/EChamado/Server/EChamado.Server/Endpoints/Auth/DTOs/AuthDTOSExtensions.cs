using EChamado.Server.Application.UseCases.Auth.Commands;

namespace EChamado.Server.Endpoints.Auth.DTOs;

/// <summary>
/// Extensões para mapeamento entre DTOs e comandos
/// </summary>
public static class AuthDTOSExtensions
{
    /// <summary>
    /// Converte LoginRequestDto para LoginUserCommand
    /// </summary>
    public static LoginUserCommand ToCommand(this LoginRequestDto requestDto)
    {
        return new LoginUserCommand(requestDto.Email, requestDto.Password);
    }

    /// <summary>
    /// Converte RegisterRequestDto para RegisterUserCommand
    /// </summary>
    public static RegisterUserCommand ToCommand(this RegisterRequestDto requestDto)
    {
        return new RegisterUserCommand(requestDto.Email, requestDto.Password);
    }
}
