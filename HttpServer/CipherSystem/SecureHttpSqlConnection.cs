using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.CipherSystem
{
    public class SecureHttpSqlConnection
    {
        public static string UsersToConnect = "Mysql_users.json";

        /// <summary>
        /// Создает новый экземпляр класса для шифврованного подключения
        /// </summary>
        public SecureHttpSqlConnection()
        {

        }


        public bool TryConnect(string address, string port, string username, string password)
        {
            try
            {
                HTTPServer.sqlServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                HTTPServer.sqlServer.Connect(new IPEndPoint(IPAddress.Parse(address), Convert.ToInt32(port)));

                HTTPServer.sqlServer.Send(Encoding.UTF8.GetBytes("REQUESTOPENKEY"));
                byte[] BOpneSqlServerKey = DynamicRecieve(HTTPServer.sqlServer);
                //byte[] BOpneSqlServerKey = new byte[1024 * 1024 * 1];
                //HTTPServer.sqlServer.Receive(BOpneSqlServerKey);
                string OpenSqlServerKey = Encoding.UTF8.GetString(BOpneSqlServerKey);
                BigInteger n = BigInteger.Parse(OpenSqlServerKey.Split("||")[0]);
                BigInteger e = BigInteger.Parse(OpenSqlServerKey.Split("||")[1]);

                string SessionKey = GenerateRandomString();
                HTTPServer.SessionKey = SessionKey;
                var cipher = RSA.RSA_Endoce(SessionKey, e, n);
                string EncodedSessionKey = "";
                foreach (var c in cipher) EncodedSessionKey += c + " ";
                HTTPServer.sqlServer.Send(Encoding.UTF8.GetBytes(EncodedSessionKey));
                //Task.Delay(2000);

                string authorizedData = username + ";" + password + ";";
                var encryptedData = Kuznechik.KuzEncript(Encoding.UTF8.GetBytes(authorizedData), Encoding.UTF8.GetBytes(SessionKey));

                HTTPServer.sqlServer.Send(encryptedData);
                byte[] msg = new byte[64];
                HTTPServer.sqlServer.Receive(msg);

                string res = Encoding.UTF8.GetString(msg);
                if (res.Contains("Success connection"))
                {
                    ConsoleText.WriteLine("Success connection", ConsoleColor.Green);
                    return true;
                }

                if (res.Contains("Connection failed"))
                {
                    ConsoleText.WriteLine("Connection failed", ConsoleColor.Red);
                    return false;
                }

                return false;
            }
            catch
            {
                ConsoleText.WriteLine("Проблемы с подключением к серверу", ConsoleColor.Red);
                return false;
            }
        }

        public static string GenerateRandomString()
        {
            Random random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789абвгдежзийклм" +
                "нопрстуфхцчшщъыьэюяАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮ☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼ !#$%&'()*+-,./";
            var stringChars = new char[random.Next() % 150 + 30];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }


        private int BufferSize = 16;
        private byte[] DynamicRecieve(Socket client)
        {
            try
            {
                Stopwatch timer = new Stopwatch();

                client.ReceiveTimeout = 0;
                byte[] msg = new byte[BufferSize];
                int bytes = client.Receive(msg);
                int scaleBuffer = 1;


                Task recieve = new Task(() =>
                {
                    try
                    {
                        while (bytes == BufferSize)
                        {
                            var temp = new byte[BufferSize];
                            client.ReceiveTimeout = 1000;
                            bytes = client.Receive(temp);

                            scaleBuffer++;

                            byte[] temp2 = new byte[BufferSize * scaleBuffer];

                            for (int i = 0; i < msg.Length; i++)
                                temp2[i] = msg[i];
                            for (int i = 0; i < temp.Length; i++)
                                temp2[i + msg.Length] = temp[i];


                            msg = new byte[BufferSize * scaleBuffer];
                            msg = temp2;

                            timer.Restart();
                        }
                    }
                    catch { }
                });

                timer.Start();
                recieve.Start();

                while (timer.ElapsedMilliseconds < 1000 && !recieve.IsCompleted) { }

                timer.Stop();
                timer.Reset();
                return msg;
            }
            catch { return Encoding.UTF8.GetBytes("Отключился:" + client.RemoteEndPoint.ToString()); }
        }

        /// <summary>
        /// Ждет байт сообщение от коннектора к базе данных по защищенному соединению
        /// </summary>
        /// <returns></returns>
        public byte[] RecieveMsg()
        {
            byte[] encryptedData = new byte[256];
            HTTPServer.sqlServer.Receive(encryptedData);

            string encryptedStr = Encoding.UTF8.GetString(encryptedData);
            encryptedStr = encryptedStr.Split("-||-")[0];
            byte[] onlyEncData = Encoding.UTF8.GetBytes(encryptedStr);

            byte[] msg = Kuznechik.KuzDecript(onlyEncData, Encoding.UTF8.GetBytes(HTTPServer.SessionKey));

            return msg;
        }

        public byte[] DynamicRecieve()
        {
            byte[] encryptedData = DynamicRecieve(HTTPServer.sqlServer);

            //string encryptedStr = Encoding.UTF8.GetString(encryptedData);
            //encryptedStr = encryptedStr.Split("-||-")[0];
            //byte[] onlyEncData = Encoding.UTF8.GetBytes(encryptedStr);

            byte[] msg = Kuznechik.KuzDecript(encryptedData, Encoding.UTF8.GetBytes(HTTPServer.SessionKey));

            return msg;
        }

        /// <summary>
        /// Отправляет сообщение присоединенному серверу
        /// </summary>
        /// <param name="httServer">Сокет, описывающий присоединенный сервер</param>
        /// <param name="msg">Строковое сообщение для отправки</param>
        public void SendMsg(string msg)
        {
            byte[] bmsg = Encoding.UTF8.GetBytes(msg);

            byte[] encryptedData = Kuznechik.KuzEncript(bmsg, Encoding.UTF8.GetBytes(HTTPServer.SessionKey));

            byte[] formatData = new byte[encryptedData.Length + 3];
            for (int i = 0; i < encryptedData.Length; i++) formatData[i] = encryptedData[i];
            formatData[encryptedData.Length] = 0x01;
            formatData[encryptedData.Length + 1] = 0x23;
            formatData[encryptedData.Length + 2] = 0x45;

            HTTPServer.sqlServer.Send(formatData);
        }

        /// <summary>
        /// Отправляет сообщение присоединенному серверу
        /// </summary>
        /// <param name="httServer">Сокет, описывающий присоединенный сервер</param>
        /// <param name="msg">Байт сообщение для отправки</param>
        public void SendMsg(byte[] msg)
        {
            byte[] encryptedData = Kuznechik.KuzEncript(msg, Encoding.UTF8.GetBytes(HTTPServer.SessionKey));

            byte[] formatData = new byte[encryptedData.Length + 3];
            for (int i = 0; i < encryptedData.Length; i++) formatData[i] = encryptedData[i];
            formatData[encryptedData.Length] = 0x01;
            formatData[encryptedData.Length + 1] = 0x23;
            formatData[encryptedData.Length + 2] = 0x45;

            HTTPServer.sqlServer.Send(formatData);
        }

        public void TestSend(byte[] msg)
        {
            byte[] encryptedData = Kuznechik.KuzEncript(msg, Encoding.UTF8.GetBytes(HTTPServer.SessionKey));

            byte[] formatData = new byte[encryptedData.Length + 3];
            for (int i = 0; i < encryptedData.Length; i++) formatData[i] = encryptedData[i];
            formatData[encryptedData.Length] = 0x01;
            formatData[encryptedData.Length + 1] = 0x23;
            formatData[encryptedData.Length + 2] = 0x45;

            HTTPServer.sqlServer.Send(formatData);
        }
    }
}
