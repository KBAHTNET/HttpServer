using System.Collections.Generic;
using HttpServer.Models;

namespace HttpServer.DatabaseSession
{
    public static class Messages
    {
        private static List<Message> messages = new List<Message>();

        public static List<Message> Get()
        {
            return messages;
        }

        public static void Add(string date, string author, string content)
        {
            messages.Add(new Message
            {
                Id = messages.Count + 1,
                //Date = date,
                //Author = author,
                Content = content
            });
        }

        public static void Remove(int id)
        {
            messages.Remove(messages.Find(m=>m.Id == id));
        }

        public static void UpdateMessagesList()
        {
            messages.Clear();
            messages = new List<Message>();
        }

    }
}
