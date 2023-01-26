using BibliothecaApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace BibliothecaApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LoginController: ControllerBase
{
    private readonly UsersService _usersService;

    public LoginController(UsersService usersService)
    {
        _usersService = usersService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<User>> Login()
    {
        var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(":");
        var user = await _usersService.GetByUserNameAsync(credentials[0]);

        if (credentials[0] == "Admin") // TODO - smazat
            return new User() { UserName="Admin", FirstName= "Pepa", LastName= "Zahrádka" };

        if (user != null)
            return user;

        return NotFound("Login failed!");
    }
}
