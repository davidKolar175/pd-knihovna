using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace BibliothecaApi;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {

    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? userName;

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(":");
            userName = credentials[0];
            var password = credentials.Last();

            if (userName != "haha")
                throw new ArgumentException("Invalid credentials!");
        }
        catch (Exception ex)
        {
            return await Task.FromResult(AuthenticateResult.Fail(ex));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName)
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principanl = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principanl, Scheme.Name);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
