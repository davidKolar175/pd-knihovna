using BibliothecaApi.Models;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;
using System.Security.Cryptography;
using System.Text;

namespace BookStoreApi.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UsersService(IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(bookStoreDatabaseSettings.Value.UsersCollectionName);
    }

    public async Task<List<User>> GetAsync(string? firstName, string? lastName, string? address, string? nin, string? sortBy)
    {
        var filter = Builders<User>.Filter.Empty;

        if (!string.IsNullOrEmpty(firstName))
            filter &= Builders<User>.Filter.Regex(nameof(User.FirstName), new BsonRegularExpression(firstName, "i"));

        if (!string.IsNullOrEmpty(lastName))
            filter &= Builders<User>.Filter.Regex(nameof(User.LastName), new BsonRegularExpression(lastName, "i"));

        if (!string.IsNullOrEmpty(address))
            filter &= Builders<User>.Filter.Regex(nameof(User.Address), new BsonRegularExpression(address, "i"));

        if (!string.IsNullOrEmpty(nin))
            filter &= Builders<User>.Filter.Regex(nameof(User.NIN), new BsonRegularExpression(nin, "i"));

        var itemsFluentFind = _usersCollection.Find(filter);

        if (string.IsNullOrEmpty(sortBy))
            return await itemsFluentFind.ToListAsync();

        return await itemsFluentFind.Sort(GetSortByFunction(sortBy)).ToListAsync();
    }

    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<List<User>> GetUnauthorizedUsersAsync() =>
        await _usersCollection.Find(x => !x.IsAuthorized).ToListAsync();

    public async Task<User?> GetByUserNameAsync(string userName) =>
        await _usersCollection.Find(x => x.UserName == userName).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) {
        newUser.Password = Sha256Hash(newUser.Password);
        await _usersCollection.InsertOneAsync(newUser);
    }

    public async Task UpdateAsync(string id, User updatedUser, bool isAdmin)
    {
        if (!isAdmin)
            updatedUser.IsAuthorized = false;

        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);
    }

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);

    public static string Sha256Hash(string value)
    {
        var sb = new StringBuilder();

        using (var hash = SHA256.Create())
        {
            var enc = Encoding.UTF8;
            var result = hash.ComputeHash(enc.GetBytes(value));

            foreach (byte b in result)
                sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }

    private static SortDefinition<User> GetSortByFunction(string sortBy)
    {
        if (sortBy == nameof(User.FirstName))
            return Builders<User>.Sort.Ascending(a => a.FirstName);

        if (sortBy == nameof(User.LastName))
            return Builders<User>.Sort.Ascending(a => a.LastName);

        if (sortBy == nameof(User.Address))
            return Builders<User>.Sort.Ascending(a => a.Address);

        if (sortBy == nameof(User.NIN))
            return Builders<User>.Sort.Ascending(a => a.NIN);

        return Builders<User>.Sort.Ascending(a => a.FirstName);
    }
}