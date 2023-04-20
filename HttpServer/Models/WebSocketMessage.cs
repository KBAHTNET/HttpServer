using System;
using System.Collections.Generic;
using System.Text;

namespace HttpServer.Models
{
    public class WebSocketMessage
    {
        public enum MessageType
        {
            Handshake,
            Message,
            Audio,
            Video,
            Command,
            Other
        }
        public MessageType Type { get; set; }
        public string Content { get; set; }
    }
}
