using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.Threading.Tasks;
using EChamado.Server.Domain.Domains.Identities;

namespace Echamado.Auth.Controllers
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public IActionResult Authorize()
        {
            var redirectUri = "https://localhost:7296/connect/authorize" + Request.QueryString; // Redirect to EChamado.Server
            return Redirect(redirectUri);
        }
    }
}