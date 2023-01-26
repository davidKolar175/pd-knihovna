using MongoDB.Bson.Serialization.Attributes;

namespace BibliothecaApi.Models
{
    public class BorrowedBook
    {
        public string UserId { get; set; }

        public string BookId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DueDate { get; set; }
    }
}
