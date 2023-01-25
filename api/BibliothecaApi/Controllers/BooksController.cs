using BibliothecaApi.Models;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksService _booksService;

    public BooksController(BooksService booksService) => _booksService = booksService;

    [HttpGet]
    public async Task<List<Book>> Get(string? name, string? author, int? year) => await _booksService.GetAsync(name, author, year);

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost("~/BorrowBook")]
    public async Task<IActionResult> BorrowBook()
    {
        /* Tady bude logika půjčení. Databáze bude muset určit, jestl si uživatel bude moct půjčit knihu nebo ne. */
        await Task.Delay(0);
        return Ok();
    }

    [HttpPost("~/ReturnBook")]
    public async Task<IActionResult> ReturnBook()
    {
        /* Tady bude logika vrácení. Uživatelé můžou vrátit knihy dřív, než vyprší 6 dnů. */
        await Task.Delay(0);
        return Ok();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Post(Book newBook)
    {
        await _booksService.CreateAsync(newBook);

        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id:length(24)}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, Book updatedBook)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        updatedBook.Id = book.Id;

        await _booksService.UpdateAsync(id, updatedBook);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        /* TODO - není možné mazat knihy, které má někdo půjčené! Logika by asi měla být uvnitř book service. */
        await _booksService.RemoveAsync(id);

        return NoContent();
    }
}