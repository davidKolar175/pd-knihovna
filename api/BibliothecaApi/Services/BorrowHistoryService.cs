using BibliothecaApi.Models;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookStoreApi.Services
{
    public class BorrowHistoryService
    {
        private readonly IMongoCollection<BorrowHistory> _borrowHistoryCollection;

        public BorrowHistoryService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
            _borrowHistoryCollection = mongoDatabase.GetCollection<BorrowHistory>(bookStoreDatabaseSettings.Value.BorrowHistoryCollectionName);
        }

        public async Task<BorrowHistory> GetAsync(string bookId, string userId) =>
            await _borrowHistoryCollection.Find(bb => bb.UserId == userId && bb.BookId == bookId).FirstOrDefaultAsync();

        public async Task<List<BorrowHistory>> GetBorrowHistoryByUserAsync(string userId) =>
            await _borrowHistoryCollection.Find(x => x.UserId == userId).ToListAsync();

        public async Task<List<BorrowHistory>> GetBorrowHistoryByBookAsync(string bookId) =>
            await _borrowHistoryCollection.Find(x => x.BookId == bookId).ToListAsync();

        public async Task CreateAsync(BorrowHistory newBook) =>
            await _borrowHistoryCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string bookId, BorrowHistory updatedBook) =>
            await _borrowHistoryCollection.ReplaceOneAsync(x => x.BookId == bookId, updatedBook);

        public async Task RemoveAsync(string bookId) =>
            await _borrowHistoryCollection.DeleteOneAsync(x => x.BookId == bookId);
    }
}
