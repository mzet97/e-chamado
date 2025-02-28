namespace EChamado.Shared.ViewModels.Auth;

public class LoginResponseViewModel
{
    public string AccessToken { get; set; } = string.Empty;
    public double ExpiresIn { get; set; }
    public UserTokenViewModel UserToken { get; set; } = new UserTokenViewModel();
}
