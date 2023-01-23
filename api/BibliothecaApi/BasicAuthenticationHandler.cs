using BibliothecaApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace BibliothecaApi;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly UsersService _userService;

    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, UsersService userService) : base(options, logger, encoder, clock)
    {
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        User? user;

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(":");
            user = await _userService.GetByUserNameAsync(credentials[0]);
            var password = credentials.Last();

            if (user == null)
                throw new ArgumentException("No username found!");

            if (user.Password != UsersService.Sha256Hash(password))
                throw new ArgumentException("Invalid credentials!");

            if (user.IsBanned)
                throw new Exception("User is banned!");
        }
        catch (Exception ex)
        {
            return await Task.FromResult(AuthenticateResult.Fail(ex));
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName)
        };

        if (user.IsAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principanl = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principanl, Scheme.Name);

        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
