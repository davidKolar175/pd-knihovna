using BibliothecaApi.Models;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookStoreApi.Services;

public class BooksService
{
    private readonly IMongoCollection<Book> _booksCollection;

    public BooksService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
        _booksCollection = mongoDatabase.GetCollection<Book>(bookStoreDatabaseSettings.Value.BooksCollectionName);
    }

    public async Task<List<Book>> GetAsync(string? name, string? author, int? year, string? sortBy)
    {
        var filter = Builders<Book>.Filter.Empty;

        if (!string.IsNullOrEmpty(name))
            filter &= Builders<Book>.Filter.Regex(nameof(Book.BookName), new BsonRegularExpression(name, "i"));

        if (!string.IsNullOrEmpty(author))
            filter &= Builders<Book>.Filter.Regex(nameof(Book.Author), new BsonRegularExpression(author, "i"));

        if (year is not null)
            filter &= Builders<Book>.Filter.Eq(x => x.Published, year);

        var itemsFluentFind = _booksCollection.Find(filter);

        if (string.IsNullOrEmpty(sortBy))
            return await itemsFluentFind.ToListAsync();

        return await itemsFluentFind.Sort(GetSortByFunction(sortBy)).ToListAsync();
    }

    public async Task<Book?> GetAsync(string id) =>
        await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Book newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _booksCollection.DeleteOneAsync(x => x.Id == id);

    private static SortDefinition<Book> GetSortByFunction(string sortBy)
    {
        if (sortBy == nameof(Book.BookName))
            return Builders<Book>.Sort.Ascending(a => a.BookName);

        if (sortBy == nameof(Book.Author))
            return Builders<Book>.Sort.Ascending(a => a.Author);

        if (sortBy == nameof(Book.Published))
            return Builders<Book>.Sort.Ascending(a => a.Published);

        return Builders<Book>.Sort.Ascending(a => a.BookName);
    }
}