namespace EChamado.Shared.Shared.Settings;

public class ClientSettings
{
    public IEnumerable<ClientData> Clients { get; set; } = Enumerable.Empty<ClientData>();
}

public class ClientData
{
    public string RedirectUris { get; set; } = string.Empty;
    public string PostLogoutRedirectUris { get; set; } = string.Empty;
}