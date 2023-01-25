﻿using BibliothecaApi.Models;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
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

    public async Task<List<User>> GetAsync(string? name, string? lastName, string? address, string? nin)
    {
        var filter = Builders<User>.Filter.Empty;

        if (!string.IsNullOrEmpty(name))
            filter &= Builders<User>.Filter.Eq(x => x.UserName, name);

        if (!string.IsNullOrEmpty(lastName))
            filter &= Builders<User>.Filter.Eq(x => x.UserName, lastName);

        if (!string.IsNullOrEmpty(address))
            filter &= Builders<User>.Filter.Eq(x => x.UserName, address);

        if (!string.IsNullOrEmpty(nin))
            filter &= Builders<User>.Filter.Eq(x => x.UserName, nin);

        return await _usersCollection.Find(filter).ToListAsync();
    }

    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task<User?> GetByUserNameAsync(string userName) =>
        await _usersCollection.Find(x => x.UserName == userName).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) {
        newUser.Password = Sha256Hash(newUser.Password);
        await _usersCollection.InsertOneAsync(newUser);
    }

    public async Task UpdateAsync(string id, User updatedUser, bool isAdmin) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

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
}