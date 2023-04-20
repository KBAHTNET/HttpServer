using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public static class HTTPServerCommands
    {
        public static List<string> Commands = new List<string> 
        {
            "help",
            "Pause",
            "Run",
            "Exit",
            "Exclude",
            "Show Excluded",
            "Include",
            "SQL:",
            "SQL.Connect(",
            "clear"
        };
        public static List<string> ExcludedIpAdresses = new List<string>();
        public static void Init(string command)
        {
            if (command.Split(' ')[0] == "help" || command == "?" || command == "-h")
            {
                ConsoleText.WriteLine(new ConsoleTextColor("Pause", ConsoleColor.Cyan), new ConsoleTextColor(" - для временной остановки работы сервера", ConsoleColor.White));
                ConsoleText.WriteLine(new ConsoleTextColor("Run", ConsoleColor.Cyan), new ConsoleTextColor(" - для возобновления работы сервера", ConsoleColor.White));
                ConsoleText.WriteLine(new ConsoleTextColor("Exit", ConsoleColor.Cyan), new ConsoleTextColor(" - для выхода и выключения сервера", ConsoleColor.White));

                ConsoleText.WriteLine(new ConsoleTextColor("Exclude", ConsoleColor.Cyan),
                    new ConsoleTextColor(" [ip] --> пример -> Exclude 127.0.0.1 - " +
                    "для исключения обработки запросов от определенного IP\n" +
                    "Для корректной работы приостановите работу сервера командой Pause", ConsoleColor.White));

                ConsoleText.WriteLine(new ConsoleTextColor("Include", ConsoleColor.Cyan),
                    new ConsoleTextColor(" [ip] --> пример -> Include 127.0.0.1 - " +
                    "для включения в обработку запросов от определенного IP, если он был ранее исключен\n" +
                    "Для корректной работы приостановите работу сервера командой Pause", ConsoleColor.White));

                ConsoleText.WriteLine(new ConsoleTextColor("SQL:<coomand>", ConsoleColor.Cyan), new ConsoleTextColor(" - выполнить команду на подключеной базе данных", ConsoleColor.White));
                Console.WriteLine("GENERATE_NEW_RSA_PAIR - генерирует новые пары (n,e) (n,d)");
            }
            if (command.Split(' ')[0] == "Pause")
            {
                HTTPServer.IsRunning = false;
                Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + ":Работа сервера приостановлена...");
            }
            if (command.Split(' ')[0] == "Run")
            {
                HTTPServer.IsRunning = true;
                Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + ":Работа сервера возобновлена...");
            }
            if (command.Split(' ')[0] == "Exit")
            {
                HTTPServer.IsRunning = false;
                HTTPServer.socket.Close();
                Environment.Exit(0);
            }
            if (command.Split(' ')[0] == "Exclude")
            {
                try
                {
                    string ExIpAddress = command.Split(' ')[1];
                    string lastOktet = ExIpAddress.Split('.')[3];
                    ExcludedIpAdresses.Add(ExIpAddress);
                    Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + ":IP[" + ExIpAddress + "] был исключен");
                }
                catch
                {
                    Console.WriteLine("Неверные параметры");
                }
            }
            if (command.Split(' ')[0] == "Include")
            {
                try
                {
                    string ExIpAddress = command.Split(' ')[1];
                    string lastOktet = ExIpAddress.Split('.')[3];
                    ExcludedIpAdresses.Remove(ExIpAddress);
                    Console.WriteLine(DateTime.Now.ToString("hh:mm:ss") + ":IP[" + ExIpAddress + "] был включен");
                }
                catch
                {
                    Console.WriteLine("Неверные параметры");
                }
            }
            if (command.Contains("Show Excluded"))
            {
                if (ExcludedIpAdresses.Count > 0)
                {
                    Console.WriteLine("Список исключенных IP:");
                    foreach (var ip in ExcludedIpAdresses)
                    {
                        Console.WriteLine(ip);
                    }
                }
                else Console.WriteLine("Список исключенных IP пуст");
            }
            if (command == "GENERATE_NEW_RSA_PAIR")
            {
                CipherSystem.RSA.GeneratePair();
            }
            if (command.Contains("TEST"))
            {
                string msg = "Moscow...11☺2☻3♥";
                HTTPServer.secureSqlConnection.TestSend(Encoding.UTF8.GetBytes(msg));
                Console.WriteLine(msg);
            }
            if (command.Contains("SQL:"))
            {
                string sql_command = command.Split(':')[1];
                if (sql_command[0] == ' ')
                {
                    string temp = "";
                    for (int i = 1; i < sql_command.Length - 1; i++)
                    {
                        temp += sql_command[i];
                    }
                    sql_command = temp;
                }
                if (sql_command[sql_command.Length - 1] != ';') sql_command += ';';

                HTTPServer.secureSqlConnection.SendMsg(sql_command);

                byte[] bresponse = HTTPServer.secureSqlConnection.DynamicRecieve();
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("      ");
                Console.SetCursorPosition(0, Console.CursorTop);

                string response = Encoding.UTF8.GetString(bresponse);
                if (response.Contains("Error:\n"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(response.Split('\n')[1]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine(response);
                }

                //Console.ForegroundColor = ConsoleColor.Blue;
                //Console.Write("Admin#");
                //Console.ForegroundColor = ConsoleColor.White;

                //try
                //{

                //    HTTPServer.sqlServer.Send(Encoding.UTF8.GetBytes(sql_command));

                //    HTTPServer.sqlServer.BeginReceive(res, 0, res.Length, SocketFlags.None, RecieveMessageSql, null);

                //}
                //catch
                //{

                //    HTTPServer.sqlServer.Dispose();
                //    HTTPServer.sqlServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //    HTTPServer.sqlServer.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999));

                //    HTTPServer.sqlServer.Send(Encoding.UTF8.GetBytes(sql_command));

                //    HTTPServer.sqlServer.BeginReceive(res, 0, res.Length, SocketFlags.None, RecieveMessageSql, null);
                //}
            }
            if (command.Contains("SQL.Connect("))
            {
                try
                {
                    string args = command.Split('(')[1].Split(')')[0];
                    string address = args.Split(';')[0];
                    string port = args.Split(';')[1];
                    string username = args.Split(';')[2];
                    //string password = args.Split(';')[3];

                    //ConsoleText.ReWrite("Пароль:", "Admin#", ConsoleColor.Green, ConsoleColor.Blue);
                    ConsoleText.WriteFromStart(new ConsoleTextColor("Пароль:", ConsoleColor.Green));

                    string password = "";
                    while (true)
                    {

                        var key = Console.ReadKey(true);//не отображаем клавишу - true

                        if (key.Key == ConsoleKey.Enter) break; //enter - выходим из цикла

                        Console.Write("*");//рисуем звезду вместо нее
                        password += key.KeyChar; //копим в пароль символы
                    }
                    Console.WriteLine();
                    HTTPServer.secureSqlConnection = new CipherSystem.SecureHttpSqlConnection();
                    HTTPServer.secureSqlConnection.TryConnect(address, port, username, password);
                    
                }
                catch
                {
                    ConsoleText.WriteLine("Неверная команда для подключения", ConsoleColor.Red);
                }

            }
            if (command == "clear")
            {
                Console.Clear();
            }
        }


        private static byte[] res = new byte[1024];
        private static void RecieveMessageSql(IAsyncResult ar)
        {
            try
            {
                int bytes = HTTPServer.sqlServer.EndReceive(ar);

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write("      ");
                Console.SetCursorPosition(0, Console.CursorTop);

                string response = Encoding.UTF8.GetString(res);
                if (response.Contains("Error:\n"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(response.Split('\n')[1]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.WriteLine(response);
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Admin#");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch { }
        }
    }
}
