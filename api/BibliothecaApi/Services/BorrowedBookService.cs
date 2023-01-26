using BibliothecaApi.Models;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services
{
    public class BorrowedBookService
    {
        private readonly IMongoCollection<BorrowedBook> _borrowedBooksCollection;

        public BorrowedBookService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
            _borrowedBooksCollection = mongoDatabase.GetCollection<BorrowedBook>(bookStoreDatabaseSettings.Value.BorrowedBooksCollectionName);
        }

        public async Task CreateAsync(BorrowedBook newBook) =>
            await _borrowedBooksCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, BorrowedBook updatedBook) =>
            await _borrowedBooksCollection.ReplaceOneAsync(x => x.BookId == id, updatedBook);

        public async Task RemoveAsync(string id) =>
            await _borrowedBooksCollection.DeleteOneAsync(x => x.BookId == id);
    }
}
