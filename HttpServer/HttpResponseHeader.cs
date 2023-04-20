using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpServer
{
    public class HttpResponseHeader
    {
        public enum HttpResponseStatus
        {
            OK_200,
            NOT_FOUND_404
        }

        public enum HttpVersion
        {
            HTTP_1_0,
            HTTP_1_1
        }

        public enum ContentType
        {
            html,
            css,
            js,
            txt,
            json,
            jpeg,
            gif,
            ico,
            png,
            webp,
            flv,
            video_mpeg,
            video_mp4,
            video_ogg,
            video_webm,
            audio_mp4,
            audio_ogg,
            audio_mpeg,
            audio_webm,
            wav,
            svg,
            bin,
            ttf,
            woff,
            woff2,
            none
        }

        public enum ConnectionType
        {
            keep_alive,
            close
        }

        private Dictionary<HttpResponseStatus, string> HttpStatusDictionary = new Dictionary<HttpResponseStatus, string>
        {
            {HttpResponseStatus.OK_200,"200 OK" },
            { HttpResponseStatus.NOT_FOUND_404, "404 Not Found"}
        };

        private Dictionary<HttpVersion, string> HttpVersionDictionary = new Dictionary<HttpVersion, string>
        {
            { HttpVersion.HTTP_1_0, "HTTP/1.0" },
            { HttpVersion.HTTP_1_1, "HTTP/1.1" },
        };

        private Dictionary<ContentType, string> HttpContentTypeDictionary = new Dictionary<ContentType, string>
        {
            {ContentType.html, "text/html" },
            {ContentType.css, "text/css" },
            {ContentType.js,"text/javascript" },
            {ContentType.txt,"text/plain" },
            {ContentType.json,"application/json" },
            {ContentType.ico,"image/vnd.microsoft.icon" },
            {ContentType.jpeg,"image/jpeg" },
            {ContentType.png,"image/png" },
            {ContentType.gif,"image/gif" },
            {ContentType.webp,"image/webp" },
            {ContentType.wav,"audio/vnd.wave" },
            {ContentType.audio_mp4,"audio/mp4" },
            {ContentType.audio_ogg,"audio/ogg" },
            {ContentType.audio_mpeg,"audio/mpeg" },
            {ContentType.audio_webm,"audio/webm" },
            {ContentType.video_mp4,"video/mp4" },
            {ContentType.video_mpeg,"video/mpeg" },
            {ContentType.video_webm,"video/webm" },
            {ContentType.flv, "video/x-flv" },
            {ContentType.svg, "image/svg+xml" },
            {ContentType.bin, "application/octet-stream" },
            {ContentType.ttf, "font/ttf" },
            {ContentType.woff, "font/woff" },
            {ContentType.woff2, "font/woff2" }
        };

        private Dictionary<ConnectionType, string> HttpConnectionDictinary = new Dictionary<ConnectionType, string>
        {
            { ConnectionType.close, "close" },
            { ConnectionType.keep_alive, "keep-alive" }
        };

        public string ServerName { get; set; }

        public HttpResponseStatus Status { get; set; }
        public HttpVersion Version { get; set; }
        public ContentType HttpContentType { get; set; }
        public ConnectionType HttpConnectionType { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public bool HttpOnlyCookie { get; set; }
        public byte[] Data { get; set; }

        public Dictionary<string, string> AdditionalHeaders { get; set; }

        public HttpResponseHeader()
        {
            ServerName = HTTPServer.SERVERNAME;
            Status = HttpResponseStatus.NOT_FOUND_404;
            Version = HttpVersion.HTTP_1_1;
            HttpContentType = ContentType.html;
            HttpConnectionType = ConnectionType.close;
            Data = new byte[1];
            Cookies = new Dictionary<string, string>();
            HttpOnlyCookie = true;

            AdditionalHeaders = new Dictionary<string, string>();
        }

        public void SetData(ref byte[] newData)
        {
            Data = new byte[newData.Length];
            for (int i = 0; i < newData.Length; i++)
            {
                Data[i] = newData[i];
            }
        }

        public void SetCookie(Dictionary<string, string> cookies, bool HttpOnly = true)
        {
            Cookies = cookies;
            HttpOnlyCookie = HttpOnly;
        }

        public string Create()
        {
            string HttpResponse = HttpVersionDictionary[Version] + " " + HttpStatusDictionary[Status] + "\n";
            HttpResponse += "Server: " + ServerName + "\n";

            if (Cookies.Count > 0)
            {
                string SetCookies = "";
                foreach (var cookie in Cookies)
                {
                    SetCookies += cookie.Key + "=" + cookie.Value + "\n";
                }
                HttpResponse += SetCookies;
            }

            HttpResponse += "Content-Language: ru\n";
            HttpResponse += "Content-type: " + HttpContentTypeDictionary[HttpContentType] + "\n";
            HttpResponse += "Accept-Ranges: bytes\n";
            HttpResponse += "ContentLength: " + Data.Length.ToString() + "\n";

            if (AdditionalHeaders.Count > 0)
            {
                foreach (var header in AdditionalHeaders)
                {
                    HttpResponse += header.Key + ": " + header.Value + "\n";
                }
            }

            HttpResponse += "Connection: " + HttpConnectionDictinary[HttpConnectionType] + "\n\n";
            HttpResponse += Encoding.UTF8.GetString(Data);

            return HttpResponse;
        }

        public byte[] CreateInBytes()
        {
            string HttpResponse = HttpVersionDictionary[Version] + " " + HttpStatusDictionary[Status] + "\n";
            HttpResponse += "Server: " + ServerName + "\n";

            if (Cookies.Count > 0)
            {
                string SetCookies = "";
                foreach (var cookie in Cookies)
                {
                    SetCookies += cookie.Key + "=" + cookie.Value + "\n";
                }
                HttpResponse += SetCookies;
            }

            HttpResponse += "Content-Language: ru\n";
            if (HttpContentType != ContentType.none)
                HttpResponse += "Content-type: " + HttpContentTypeDictionary[HttpContentType] + "\n";
            HttpResponse += "Accept-Ranges: bytes\n";
            HttpResponse += "ContentLength: " + Data.Length.ToString() + "\n";

            if (AdditionalHeaders.Count > 0)
            {
                foreach (var header in AdditionalHeaders)
                {
                    HttpResponse += header.Key + ": " + header.Value + "\n";
                }
            }

            HttpResponse += "Connection: " + HttpConnectionDictinary[HttpConnectionType] + "\n\n";

            //byte[] data = Encoding.UTF8.GetBytes(HttpResponse).Concat(Data).ToArray();

            //Concatination

            byte[] data = new byte[Encoding.UTF8.GetBytes(HttpResponse).Length + Data.Length];
            byte[] rh = Encoding.UTF8.GetBytes(HttpResponse);
            for (int i = 0; i < rh.Length; i++) data[i] = rh[i];
            for (int i = rh.Length; i < rh.Length + Data.Length; i++) data[i] = Data[i - rh.Length];

            return data;
        }

    }
}
