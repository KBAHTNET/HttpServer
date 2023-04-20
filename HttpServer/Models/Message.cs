using System;

namespace HttpServer.Models
{
    public class Message
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public long AuthorId { get; set; }
    }
}
