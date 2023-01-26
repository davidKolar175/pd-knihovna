using MongoDB.Bson.Serialization.Attributes;

namespace BibliothecaApi.Models
{
    public class BorrowedBook
    {
        public string UserId { get; set; } = null!;

        public string BookId { get; set; } = null!;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DueDate { get; set; }
    }
}
