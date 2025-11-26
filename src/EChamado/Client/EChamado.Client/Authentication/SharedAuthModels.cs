using System.Text.Json.Serialization;

namespace EChamado.Client.Authentication;

public class OpenIddictUserInfo
{
    public bool IsAuthenticated { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public string[]? Roles { get; set; }
}

/// <summary>
/// OAuth2 Token Response - Properties mapped to snake_case JSON format
/// </summary>
public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [JsonPropertyName("id_token")]
    public string? IdToken { get; set; }

    [JsonPropertyName("scope")]
    public string? Scope { get; set; }
}