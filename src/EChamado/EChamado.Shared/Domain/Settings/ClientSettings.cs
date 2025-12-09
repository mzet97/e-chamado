namespace EChamado.Shared.Domain.Settings;

public class ClientSettings
{
    public IEnumerable<ClientData> Clients { get; set; }
}

public class ClientData
{
    public string RedirectUris { get; set; }
    public string PostLogoutRedirectUris { get; set; }
}