using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BibliothecaApi.Models
{
    public class BorrowedBook
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserId { get; set; }

        public string BookId { get; set; } = null!;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BorrowedDate { get; set; }
    }
}
