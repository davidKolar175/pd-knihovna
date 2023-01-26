using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BibliothecaApi.Models;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string BookName { get; set; } = null!;

    public string Author { get; set; } = null!;

    public int NumberOfPages { get; set; }

    public int Published { get; set; }

    public int Copies { get; set; }
}