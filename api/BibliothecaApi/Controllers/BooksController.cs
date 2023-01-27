using BibliothecaApi.Models;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Net;

namespace BookStoreApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksService _booksService;
    private readonly UsersService _usersService;
    private readonly BorrowedBookService _borrowedBookService;
    private readonly BorrowHistoryService _borrowHistoryService;

    public BooksController(BooksService booksService, UsersService usersService, BorrowedBookService borrowedBookService, BorrowHistoryService borrowHistoryService)
    {
        _booksService = booksService;
        _usersService = usersService;
        _borrowedBookService = borrowedBookService;
        _borrowHistoryService = borrowHistoryService;
    }

    [HttpGet]
    public async Task<List<Book>> Get(string? bookName, string? author, int? year, string? sortBy) => await _booksService.GetAsync(bookName, author, year, sortBy);

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
            return NotFound();

        return book;
    }

    [HttpGet("GetBorrowedBooks")]
    public async Task<List<Book>> GetBorrowedBooks(string userId)
    {
        var borrowedBooks = await _borrowedBookService.GetBorrowedBooksByUserAsync(userId);
        var books = new List<Book>();

        foreach (var borrowedBook in borrowedBooks)
        {
            var book = await _booksService.GetAsync(borrowedBook.BookId);
            if (book is null)
                throw new Exception("Book not found!");
            books.Add(book);
        }

        return books;
    }

    [HttpGet("GetBorrowHistory")]
    public async Task<List<BorrowHistory>> GetBorrowHistory(string userId)
    {
        var borrowHistory = await _borrowHistoryService.GetBorrowHistoryByUserAsync(userId);
        var books = new List<BorrowHistory>();

        foreach (var borrowedBook in borrowHistory)
        {
            var book = await _borrowHistoryService.GetAsync(borrowedBook.BookId, userId);
            if (book is null)
                throw new Exception("Book not found!");
            books.Add(book);
        }

        return books;
    }

    [HttpPost("BorrowBook")]
    public async Task<IActionResult> BorrowBook(string userId, string bookId)
    {
        /* Tady bude logika půjčení. Databáze bude muset určit, jestl si uživatel bude moct půjčit knihu nebo ne. */
        var user = await _usersService.GetAsync(userId);
        var book = await _booksService.GetAsync(bookId);

        if (user == null || book == null)
        {
            throw new Exception("User or book not found");
        }
        if (user.BorrowedBooks?.Count >= 6)
        {
            throw new Exception("User has already borrowed the maximum number of books");
        }
        if (user.BorrowedBooks.Exists(x => x.BookId == bookId))
        {
            throw new Exception("User has already borrowed this book");
        }
        if (book.Copies <= 0)
        {
            throw new Exception("There are no more copies of this book available");
        }

        var borrowedBook = new BorrowedBook
        {
            UserId = userId,
            BookId = bookId,
            BorrowedDate = DateTime.Now
        };

        var historyItem = new BorrowHistory
        {
            UserId = borrowedBook.UserId,
            BookId = borrowedBook.BookId,
            BorrowDate = DateTime.Now,
            ReturnDate = DateTime.Now.AddDays(6)
        };

        user.BorrowedBooks?.Add(borrowedBook);
        book.Copies = book.Copies - 1;

        await _borrowedBookService.CreateAsync(borrowedBook);
        await _borrowHistoryService.CreateAsync(historyItem);
        await _usersService.UpdateAsync(userId, user, user.IsAdmin);
        await _booksService.UpdateAsync(bookId, book);
        return Ok();
    }

    [HttpPost("ReturnBook")]
    public async Task<IActionResult> ReturnBook(string userId, string bookId)
    {
        /* Tady bude logika vrácení. Uživatelé můžou vrátit knihy dřív, než vyprší 6 dnů. */
        var user = await _usersService.GetAsync(userId);
        var book = await _booksService.GetAsync(bookId);
        var borrowedBook = await _borrowedBookService.GetAsync(bookId,userId);
        var borrowHistoryBook = await _borrowHistoryService.GetAsync(bookId, userId);

        if (user == null || book == null || borrowedBook == null)
        {
            throw new Exception("User, book, or borrowed book not found");
        }

        var borrowItem = user.BorrowedBooks?.Find( x => x.BookId == borrowedBook.BookId);
        borrowHistoryBook.ReturnDate = DateTime.Now;
        user.BorrowedBooks?.Remove(borrowItem);
        book.Copies = book.Copies + 1;

        await _borrowedBookService.RemoveAsync(borrowItem.BookId);
        await _borrowHistoryService.UpdateAsync(bookId, borrowHistoryBook);
        await _usersService.UpdateAsync(userId, user, user.IsAdmin);
        await _booksService.UpdateAsync(bookId, book);
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

        /* Není možné mazat knihy, které má někdo půjčené! Logika by asi měla být uvnitř book service. */
        var borrowedBooks = await _borrowedBookService.GetBorrowedBooksByBookAsync(id);
        if (borrowedBooks.Exists(x => x.BookId == id))
            throw new Exception("You can't delete borrowed book.");

        await _booksService.RemoveAsync(id);

        return NoContent();
    }
}