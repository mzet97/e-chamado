using Microsoft.JSInterop;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EChamado.Client.Authentication;

public class AuthTokenHandler : DelegatingHandler
{
    private readonly IJSRuntime _js;

    public AuthTokenHandler(IJSRuntime js)
    {
        _js = js;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Get token from localStorage
            var token = await _js.InvokeAsync<string?>("localStorage.getItem", "authToken");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            // Log warning but don't fail the request
            Console.WriteLine($"Failed to get auth token: {ex.Message}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}