using BibliothecaApi.Models;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStoreApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;
    private readonly BooksService _booksService;
    private readonly BorrowedBookService _borrowedBookService;

    public UsersController(UsersService usersService, BooksService bookService, BorrowedBookService borrowedBookService)
    {
        _usersService = usersService;
        _booksService = bookService;
        _borrowedBookService = borrowedBookService;
    }

    [HttpGet]
    public async Task<List<User>> Get(string? firstName, string? lastName, string? address, string? nin, string? sortby) =>
        await _usersService.GetAsync(firstName, lastName, address, nin, sortby);

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
            return NotFound();

        return user;
    }

    [HttpGet("GetReadersOfBook")]
    public async Task<List<User>> GetReadersOfBook(string bookId)
    {
        var borrowedBooks = await _borrowedBookService.GetBorrowedBooksByBookAsync(bookId);
        var readers = new List<User>();

        foreach (var reader in borrowedBooks)
        {
            var user = await _usersService.GetAsync(reader.UserId);
            if (user is null)
                throw new Exception("User not found!");
            readers.Add(user);
        }

        return readers;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("GetUnauthorizedUsers")]
    public async Task<List<User>> GetUnauthorizedUsers() =>
        await _usersService.GetUnauthorizedUsersAsync();

    [HttpPost]
    public async Task<IActionResult> Post(User newUser)
    {
        await _usersService.CreateAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var isAdmin = false;
        var identity = HttpContext.User.Identity as ClaimsIdentity;

        if (identity != null)
        {
            var roleClaim = identity.Claims.First(x => x.Type == ClaimTypes.Role);
            if (roleClaim.Value == "Admin")
                isAdmin = true;

        }

        var user = await _usersService.GetAsync(id);

        if (user is null)
            return NotFound();

        updatedUser.Id = user.Id;

        await _usersService.UpdateAsync(id, updatedUser, isAdmin);

        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
            return NotFound();

        await _usersService.RemoveAsync(id);

        return NoContent();
    }
}