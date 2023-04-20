using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    public class HttpRequestHeader
    {
        public string Url;
        public string ClientDevice;
        public string ClientIp;
        public string Host;
        public Dictionary<string, string> Arguments;
        public Dictionary<string, string> Cookies;
        public HttpType Type;
        public ConnectionType Connection;
        public byte[] Body;
        public string StrBody;

        public enum HttpType
        {
            GET,
            POST
        }

        public enum ConnectionType
        {
            keep_alive,
            close,
            unknow
        }

        public HttpRequestHeader(byte[] msg, int bytes, Socket client)
        {
            //int bufferSize = Request.BufferSize;

            //Console.WriteLine("\n" + Encoding.UTF8.GetString(msg) + "\n");

            //if (bytes <= bufferSize)
            //{
                string request = Encoding.UTF8.GetString(msg);

                ParseType(request);
                ParseUrl(request);
                ParseHost(request);
                ParseConnection(request);
                ParseClientDevice(request);
                ParseClientIp(client);
                ParseCookie(request);
                ParseBody(request);
                ParseArguments(request);
            //}
            //else Console.WriteLine("Переполнение буфера");
        }

        public string GetRequestString()
        {
            string request = "TYPE:" + Type.ToString() + "\n";
            request += "Connection:" + Connection.ToString() + "\n";
            request += "Url:" + Url.ToString() + "\n";
            request += "Host:" + Host.ToString() + "\n";
            request += "User-Agent:" + ClientDevice.ToString() + "\n";
            request += "Ip:" + ClientIp.ToString() + "\n";
            try 
            {
                string cookies = "";

                foreach(var cookie in Cookies)
                {
                    cookies += cookie.Key + "=" + cookie.Value + ";"; 
                }

                request += "Cookies:" + cookies + "\n"; 
            }
            catch { }
            try 
            {
                string arguments = "";

                foreach (var argument in Arguments)
                {
                    arguments += argument.Key + "=" + argument.Value + "\n";
                }

                request += "Arguments:\n" + arguments + "\n";
            }
            catch { }

            try
            {
                request += "Body:\n" + StrBody + "\n";
            }
            catch { }

            return request;
        }

        private void ParseType(string request)
        {
            if (request.Split(' ')[0] == "GET") Type = HttpType.GET;
            if (request.Split(' ')[0] == "POST") Type = HttpType.POST;
        }

        private void ParseUrl(string request)
        {
            try { Url = request.Split(' ')[1]; }
            catch { Url = "/"; }
        }

        private void ParseHost(string request)
        {
            int FindIndex = request.IndexOf("Host:");
            Host = "";
            if (FindIndex != -1)
            {
                for (int i = FindIndex; i < request.Length; i++)
                {
                    if (request[i] == '\n')
                    {
                        Host = Host.Replace(" ", "");
                        Host = Host.Replace("Host:", "");
                        break;
                    }
                    Host += request[i].ToString();
                }
            }
            else Host = "Неизвестно";
        }

        private void ParseConnection(string request)
        {
            int FindIndex = request.IndexOf("Connection:");
            if (FindIndex != -1)
            {
                string connection = "";
                for (int i = FindIndex; i < request.Length; i++)
                {
                    if (request[i] == '\n')
                    {
                        connection = connection.Replace(" ", "");
                        connection = connection.Replace("Connection:", "");
                        if (connection.Contains("keep-alive")) Connection = ConnectionType.keep_alive;
                        else if (connection.Contains("close")) Connection = ConnectionType.close;
                        else Connection = ConnectionType.unknow;
                        break;
                    }
                    connection += request[i].ToString();
                }
            }
            else Connection = ConnectionType.unknow;
        }

        private void ParseClientDevice(string request)
        {
            int FindIndex = request.IndexOf("User-Agent:");
            ClientDevice = "";
            if (FindIndex != -1)
            {
                for (int i = FindIndex; i < request.Length; i++)
                {
                    if (request[i] == '\n')
                    {
                        ClientDevice = ClientDevice.Replace("User-Agent: ", "");
                        try { ClientDevice = ClientDevice.Split('(')[1].Split(')')[0]; }
                        catch { }
                        break;
                    }
                    ClientDevice += request[i].ToString();
                }
            }
            else ClientDevice = "Неизвестно";
        }

        private void ParseClientIp(Socket client)
        {
            try { ClientIp = client.RemoteEndPoint.ToString(); }
            catch { ClientIp = "Неизвестно"; }
        }

        private void ParseBody(string request)
        {
            string body = "";
            int FindIndex = request.IndexOf("\r\n\r\n");
            if (FindIndex != -1)
            {
                body = request.Split("\r\n\r\n")[1];
            }
            StrBody = body;
            Body = Encoding.UTF8.GetBytes(body);
        }

        private void ParseArguments(string request)
        {
            try
            {
                Arguments = new Dictionary<string, string>();

                if (Type == HttpType.GET)
                {
                    if (Url.Contains('?'))
                    {
                        try
                        {
                            string arguments = Url.Split('?')[1];

                            foreach (string argument in arguments.Split('&'))
                            {
                                try { Arguments.Add(argument.Split('=')[0], argument.Split('=')[1]); }
                                catch { Arguments[argument.Split('=')[0]] = argument.Split('=')[1]; }
                            }
                        }
                        catch { }
                    }
                }

                if (Type == HttpType.POST)
                {
                    string arguments = request.Split('\n')[request.Split('\n').Length - 1];

                    foreach (string argument in arguments.Split('&'))
                    {
                        try { Arguments.Add(argument.Split('=')[0], argument.Split('=')[1]); }
                        catch { Arguments[argument.Split('=')[0]] = argument.Split('=')[1]; }
                    }
                }
            }
            catch { }
        }

        private void ParseCookie(string request)
        {
            Cookies = new Dictionary<string, string>();

            int FindIndex = request.IndexOf("Cookie:");

            if (FindIndex != -1)
            {
                string cookies = "";

                for (int i = FindIndex; i < request.Length; i++)
                {
                    if (request[i] == '\n')
                    {
                        cookies = cookies.Replace("Cookie: ", "");
                        cookies = cookies.Replace("; ", ";");

                        foreach (string cookie in cookies.Replace("; ", ";").Split(';'))
                        {
                            try { Cookies.Add(cookie.Split('=')[0], cookie.Split('=')[1]); }
                            catch { Cookies[cookie.Split('=')[0]] = cookie.Split('=')[1]; }
                        }

                        break;
                    }
                    cookies += request[i].ToString();
                }
            }
        }
    }
}
