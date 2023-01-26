using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BibliothecaApi.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string NIN { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool IsBanned { get; set; }

    public bool IsAuthorized { get; set; }

    public bool IsAdmin { get; set; }

    public List<BorrowedBook>? BorrowedBooks = new List<BorrowedBook>();
}
