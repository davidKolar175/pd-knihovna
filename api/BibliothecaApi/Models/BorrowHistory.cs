using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BibliothecaApi.Models
{
    public class BorrowHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserId { get; set; }

        public string BookId { get; set; } = null!;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BorrowDate { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ReturnDate { get; set; }
    }
}
