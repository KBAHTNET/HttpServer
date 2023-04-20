using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using HttpServer.DatabaseSession;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Net.WebSockets;
using System.Linq;

namespace HttpServer
{
    public static class Request
    {
        public static int BufferSize = 10000000;

        private static int sec = 0;

        public static void AcceptRequest(ref Socket client)
        {
            //byte[] msg = HTTPServer.DynamicRecieve(client);
            byte[] msg = new byte[BufferSize];
            try
            {
                byte r = Convert.ToByte('|');
                for (int i = 0; i < 10000000; i += 1) msg[i] = r;
                int bytes = client.Receive(msg);
                msg = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(msg).Replace("|", ""));
                //Console.WriteLine("REQUEST\n" + Encoding.UTF8.GetString(msg));
                HttpRequestHeader requestHeader = new HttpRequestHeader(msg, msg.Length, client);
                HttpResponseHeader responseHeader = null;

                if (requestHeader.Url.Contains("save_avatar"))
                {
                    Console.WriteLine("Мы тут");
                    byte[] array = requestHeader.Body;
                    using (FileStream fs = new FileStream("NewFile.jpg", FileMode.CreateNew, FileAccess.Write))
                    {
                        fs.Write(array);
                        client.Send(Encoding.UTF8.GetBytes("Збс прошло"));
                        client.Close();
                    }
                }

                if (requestHeader.Url.Contains("/user/"))
                {
                    string sqlReq = "select * from users where nickname=" + requestHeader.Url.Split('/')[requestHeader.Url.Split('/').Length - 1];
                    HTTPServer.sqlServer.Send(Encoding.UTF8.GetBytes(sqlReq));
                    byte[] userInf = new byte[1024];
                    int userBytes = HTTPServer.sqlServer.Receive(userInf);
                    responseHeader = new HttpResponseHeader();
                    responseHeader.Data = Encoding.UTF8.GetBytes("Соси хуй и не психуй");
                    responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                    responseHeader.HttpContentType = HttpResponseHeader.ContentType.bin;
                    responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                    responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;
                    client.Send(responseHeader.CreateInBytes());
                    client.Close();
                }

                if (requestHeader.Url.Contains(".well-known/pki-validation"))
                {
                    responseHeader = new HttpResponseHeader();
                    responseHeader.Data = File.ReadAllBytes("1404FAE811446A4A7D48D00AD73B22C3.txt");
                    responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                    responseHeader.HttpContentType = HttpResponseHeader.ContentType.txt;
                    responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                    responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;
                    responseHeader.AdditionalHeaders.Add("Content-Disposition", "attachment; filename=1404FAE811446A4A7D48D00AD73B22C3.txt");
                    //Console.WriteLine(responseHeader.Create());
                    client.Send(responseHeader.CreateInBytes());
                    client.Close();
                }

                if (requestHeader.Url == "/webSocket")
                {
                    byte[] data = new byte[msg.Length];
                    //byte[] data = new byte[bytes];
                    data = msg;
                    if (new System.Text.RegularExpressions.Regex("^GET").IsMatch(Encoding.UTF8.GetString(data)))
                    {
                        const string eol = "\r\n"; // HTTP/1.1 defines the sequence CR LF as the end-of-line marker

                        byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + eol
                            + "Connection: Upgrade" + eol
                            + "Upgrade: websocket" + eol
                            + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
                                System.Security.Cryptography.SHA1.Create().ComputeHash(
                                    Encoding.UTF8.GetBytes(
                                        new System.Text.RegularExpressions.Regex("Sec-WebSocket-Key: (.*)").Match(Encoding.UTF8.GetString(data)).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                    )
                                )
                            ) + eol
                            + eol);


                        client.Send(response);

                        HTTPServer.clients.Add(client);

                        while (client.Connected)
                        {
                            byte[] recmes = new byte[1024];
                            try
                            {
                                int length = client.Receive(recmes);

                                string s = "";
                                for (int i = 0; i < recmes.Length; i++) s += Convert.ToString(recmes[i], 2);
                                Console.WriteLine(s);

                                WebSocketConnection ws = new WebSocketConnection(recmes);


                                if (ws.OPCODE == 0x9) Console.WriteLine("Ping");
                                if (ws.OPCODE == 0xA) Console.WriteLine("Pong");

                                Console.WriteLine(Encoding.UTF8.GetString(ws.MESSAGE));
                                int len = ws.MESSAGE.Length;
                                byte[] mes = new byte[len + 2];
                                mes[0] = 0x81;
                                mes[1] = BitConverter.GetBytes(len)[0];
                                for (int i = 0; i < len; i++) mes[i + 2] = ws.MESSAGE[i];

                                List<Socket> clientsRemove = new List<Socket>();
                                WebSocketConnection checkws = new WebSocketConnection(mes);
                                foreach (var c in HTTPServer.clients)
                                {
                                    if (c.RemoteEndPoint != client.RemoteEndPoint)
                                    {
                                        try
                                        {
                                            if (ws.OPCODE == 0x1)
                                            {
                                                Console.WriteLine("Отправка:" + c.RemoteEndPoint.ToString());

                                                c.Send(mes);
                                            }
                                        }
                                        catch { clientsRemove.Add(c); }
                                    }
                                }

                                foreach (var c in clientsRemove) HTTPServer.clients.Remove(c);
                            }
                            catch
                            {
                                HTTPServer.clients.Remove(client);
                                break;
                            }
                        }

                        Console.WriteLine("Disconnected{" + client.RemoteEndPoint + "}");
                        HTTPServer.clients.Remove(client);

                    }

                }

                //Console.WriteLine(requestHeader.GetRequestString());
                //Console.WriteLine(Encoding.UTF8.GetString(msg));

                string msg_bytes = "";
                string tls_header = "(1 byte)Заголовок:";
                string vers_and_len = "(2 bytes and 2 bytes)Версия и длина пакета данных:";
                string type_tls_msg = "(1 byte)Тип сообщения:";
                string len_data = "(3 bytes)Длина блока данных:";
                string client_tls_ver = "(2 bytes)Версия TLS клиента:";
                string random_bytes = "(32 bytes)Случайный байты:";
                string SessionID_len = "";
                string SessionID = "Id Session:";
                string List_cyphers_len = "";
                string Cyphers = "Шифронаборы:";
                string zip = "(2 bytes)Длина поля CompressionMethods(если 01 00, то клинет не поддерживает сжатие):";
                string extensions_len = "";
                string hyinia = "Дальше хуйня с Extensions SNI CURVE ELIPTIC и прочая хуйня";

                for (int i = 0; i < msg.Length; i++)
                {
                    if (i == 0) tls_header += Convert.ToString(msg[i], 16);
                    if (i > 0 && i < 5) vers_and_len += Convert.ToString(msg[i], 16) + " ";
                    if (i == 5) type_tls_msg += Convert.ToString(msg[i], 16);
                    if (i > 5 && i < 9) len_data += Convert.ToString(msg[i], 16) + " ";
                    if (i > 8 && i < 11) client_tls_ver += Convert.ToString(msg[i], 16) + " ";
                    if (i > 10 && i < 43) random_bytes += Convert.ToString(msg[i], 16) + " ";
                    if (i == 43) SessionID_len += Convert.ToString(msg[i], 16);

                    if (i > 43)
                    {
                        msg_bytes += Convert.ToString(msg[i], 16);
                        if (i > 43 && i <= 43 + Convert.ToInt32(SessionID_len, 16)) SessionID += Convert.ToString(msg[i], 16) + " ";

                        if (i > 43 + Convert.ToInt32(SessionID_len, 16))
                        {
                            if (i > 43 + Convert.ToInt32(SessionID_len, 16) && i <= 44 + Convert.ToInt32(SessionID_len, 16)) List_cyphers_len += Convert.ToString(msg[i], 16) + Convert.ToString(msg[i], 16);

                            if (i > 45 + Convert.ToInt32(SessionID_len, 16) + Convert.ToInt32(List_cyphers_len, 16))
                            {
                                msg_bytes += Convert.ToString(msg[i], 16) + " ";
                                if (i > 45 + Convert.ToInt32(SessionID_len, 16) && i <= 45 + Convert.ToInt32(SessionID_len, 16) + Convert.ToInt32(List_cyphers_len, 16)) Cyphers += Convert.ToString(msg[i], 16) + " ";
                                if (i > 45 + Convert.ToInt32(SessionID_len, 16) + Convert.ToInt32(List_cyphers_len, 16) && i <= 47 + Convert.ToInt32(SessionID_len, 16) + Convert.ToInt32(List_cyphers_len, 16)) zip += Convert.ToString(msg[i], 16) + " ";
                                if (i > 47 + Convert.ToInt32(SessionID_len, 16) + Convert.ToInt32(List_cyphers_len, 16) && i <= 49 + Convert.ToInt32(SessionID_len, 16) + Convert.ToInt32(List_cyphers_len, 16)) extensions_len += Convert.ToString(msg[i], 16) + " ";
                            }
                        }
                    }
                }


                //Console.WriteLine(msg_bytes);

                //Console.WriteLine(tls_header);
                //Console.WriteLine(vers_and_len);
                //Console.WriteLine(type_tls_msg);
                //Console.WriteLine(len_data);
                //Console.WriteLine(client_tls_ver);
                //Console.WriteLine(random_bytes);
                //Console.WriteLine(msg_bytes);
                //Console.WriteLine(SessionID_len + "=" + Convert.ToInt32(SessionID_len, 16));
                //Console.WriteLine(SessionID);

                //Console.WriteLine(List_cyphers_len + "=" + Convert.ToInt32(List_cyphers_len, 16));
                //Console.WriteLine(Cyphers);
                //Console.WriteLine(msg_bytes);
                //Console.WriteLine(zip);
                //Console.WriteLine(extensions_len);

                //Console.WriteLine(Encoding.UTF8.GetString(msg));

                //ConsoleText.ReWrite(Encoding.UTF8.GetString(msg), ConsoleTextSettings.UsNamPref + ConsoleText.CurrentCommand, ConsoleColor.White, ConsoleColor.Blue);

                //ConsoleText.RewriteCurrenCommandtString(Encoding.UTF8.GetString(msg), ConsoleTextSettings.RequestColor);
                //MyConsole.ReWriteCurrent(Encoding.UTF8.GetString(msg),MyConsole.TextColor, MyConsole.UP, MyConsole.UPColor);
                //MyConsole.Write(MyConsole.CurrentCommand, MyConsole.CommandColor);

                MyConsole.Write(Encoding.UTF8.GetString(msg),MyConsole.TextColor, 0, MyConsole.Top);
                MyConsole.Write(MyConsole.UP, MyConsole.UPColor, 0, MyConsole.Top);
                MyConsole.Write(MyConsole.CurrentCommand, MyConsole.TextColor, MyConsole.Left, MyConsole.Top);

                StaticFiles.Init(ref client, requestHeader.Url);
                Pages.Init(ref client, requestHeader);

                client.Close();
            }
            catch
            {
                Console.WriteLine(client.RemoteEndPoint.ToString() + " отключился");
            }

        }

        //private static byte[] RecieveMSG(ref Socket client)
        //{
        //    try
        //    {
        //        byte[] msg = new byte[BufferSize];
        //        int bytes = client.Receive(msg);
        //        int scaleBuffer = 1;
        //        while (bytes >= BufferSize)
        //        {
        //            var temp = msg;
        //            msg = new byte[BufferSize];
        //            bytes = client.Receive(msg);
        //            scaleBuffer++;

        //            byte[] temp2 = new byte[BufferSize * scaleBuffer];

        //            for (int i = 0; i < temp.Length; i++)
        //                temp2[i] = temp[i];
        //            for (int i = 0; i < msg.Length; i++)
        //                temp2[i + temp.Length] = msg[i];


        //            msg = new byte[BufferSize * scaleBuffer];
        //            msg = temp2;
        //        }
        //        return msg;
        //    }
        //    catch { return Encoding.UTF8.GetBytes("Отключился:" + client.RemoteEndPoint.ToString()); }
        //}

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
