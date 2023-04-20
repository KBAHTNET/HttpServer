using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;

namespace HttpServer
{
    class HTTPServer
    {
        public const string SERVERNAME = "KBAPK";
        public const string VERSION = "HTTP/1.1";
        public static bool IsRunning;

        public static string sqlDB = "KBAPK_DB";
        public static Socket sqlServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static CipherSystem.SecureHttpSqlConnection secureSqlConnection;
        public static string SessionKey = "";

        public static Socket socket;
        public HTTPServer(int Port)
        {
             socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
             socket.Bind(new IPEndPoint(IPAddress.Any, Port));
             socket.Listen(0);
             IsRunning = false;
        }

        int CommandsLimit = 30;
        List<string> commandsHistory = new List<string>();
        int historyOffset = 0;
        int offset = 0;

        //public static int ConsoleLeft, ConsoleTop;
        public void Start()
        {
            socket.BeginAccept(AcceptCallBack, null);
            IsRunning = true;

            ConsoleText.WriteLine("Сервер запущен", ConsoleColor.Green);

            while (true)
            {
                ConsoleText.Write(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor);
                //string command = ConsoleText.WaitForInput();
                string command = MyConsole.ReadLine();
                HTTPServerCommands.Init(command);
            }
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                var client = socket.EndAccept(ar);
                if (!HTTPServerCommands.ExcludedIpAdresses.Contains(client.RemoteEndPoint.ToString().Split(':')[0]))
                {
                    Task clientTread = new Task(async() => await ClientHandler(client));
                    clientTread.Start();
                }
                else
                {
                    ConsoleText.WriteLine("\nПопытка подключения клиента[" + client.RemoteEndPoint.ToString() + "], исключенного из обработки запросов\n", ConsoleColor.Red);
                }
            Run:
                if (IsRunning) socket.BeginAccept(AcceptCallBack, null);
                else
                {
                    while (!IsRunning) { }
                    goto Run;
                }
            }
            catch { }
        }

        public static List<Socket> clients = new List<Socket>();
        private async Task ClientHandler(Socket client)
        {
            Request.AcceptRequest(ref client);
        }

        private static int BufferSize = 16;
        public static byte[] DynamicRecieve(Socket client)
        {
            try
            {
                Stopwatch timer = new Stopwatch();

                client.ReceiveTimeout = 0;
                byte[] msg = new byte[BufferSize];
                for (int i = 0; i < BufferSize; i++) msg[i] = Convert.ToByte('|');

                int bytes = client.Receive(msg);
                int scaleBuffer = 1;


                Task recieve = new Task(() =>
                {
                    try
                    {
                        while (bytes == BufferSize)
                        {
                            byte[] temp = new byte[BufferSize];
                            for (int i = 0; i < BufferSize; i++) temp[i] = Convert.ToByte('|');
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

                string s = Encoding.UTF8.GetString(msg);
                s = s.Split('|')[0];

                msg = Encoding.UTF8.GetBytes(s);

                return msg;
            }
            catch { return Encoding.UTF8.GetBytes("Отключился:" + client.RemoteEndPoint.ToString()); }
        }
    }
}


/*                while (true)
                {
                    ConsoleTop = Console.CursorTop;
                    ConsoleLeft = Console.CursorLeft;
                    var key = Console.ReadKey();

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                        for (int i = 0; i < HTTPServerCommands.Commands.Count; i++)
                        {
                            Console.Write("                              ");
                            Console.SetCursorPosition(0, Console.CursorTop + 1);
                        }

                        Console.SetCursorPosition(ConsoleLeft, ConsoleTop);

                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                        if(command.Length > 1) commandsHistory.Add(command);
                        if (commandsHistory.Count > CommandsLimit) commandsHistory.RemoveAt(0);
                        historyOffset = 0;
                        offset = 0;
                        break;
                    }

                    if(key.Key == ConsoleKey.UpArrow)
                    {
                        Console.SetCursorPosition(ConsoleLeft, ConsoleTop);

                        historyOffset++;
                        try
                        {
                            command = commandsHistory[commandsHistory.Count - historyOffset];
                            ConsoleText.WriteFromStart(new ConsoleTextColor("                                                                                                                                                           ", ConsoleColor.White));
                            Console.SetCursorPosition(ConsoleLeft, ConsoleTop);
                            ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(command, ConsoleColor.White));
                        }
                        catch 
                        {
                            historyOffset--;
                        }

                        continue;
                    }

                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        Console.SetCursorPosition(ConsoleLeft, ConsoleTop);
                        if (historyOffset - 1 > -1) historyOffset--;
                        try
                        {
                            command = commandsHistory[commandsHistory.Count - historyOffset];
                            ConsoleText.WriteFromStart(new ConsoleTextColor("                                                                                                                                                           ", ConsoleColor.White));
                            Console.SetCursorPosition(ConsoleLeft, ConsoleTop);
                            ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(command, ConsoleColor.White));
                        }
                        catch 
                        {
                            ConsoleText.WriteFromStart(new ConsoleTextColor("                                                                                                                                                           ", ConsoleColor.White));
                            Console.SetCursorPosition(ConsoleLeft, ConsoleTop);
                            ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor));
                            command = "";
                        }
                        continue;
                    }

                    if (key.Key == ConsoleKey.LeftArrow)
                    {
                        if (Math.Abs(offset) < command.Length)
                        {
                            ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(command, ConsoleColor.White));
                            Console.SetCursorPosition(ConsoleLeft - 1, ConsoleTop);
                            offset--;
                        }
                        else
                        {
                            ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(command, ConsoleColor.White));
                            Console.SetCursorPosition(ConsoleLeft, ConsoleTop);
                        }
                        
                        continue;
                    }

                    if (key.Key == ConsoleKey.RightArrow)
                    {
                        if (offset+1 <= 0)
                        {
                            ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(command, ConsoleColor.White));
                            Console.SetCursorPosition(ConsoleLeft + 1, ConsoleTop);
                            offset++;
                        }
                        else
                        {
                            ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(command, ConsoleColor.White));
                            Console.SetCursorPosition(ConsoleLeft, ConsoleTop);
                        }
                        continue;
                    }

                    if (key.Key == ConsoleKey.Tab)
                    {
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                        for (int i = 0; i < HTTPServerCommands.Commands.Count; i++)
                        {
                            Console.Write("                              ");
                            Console.SetCursorPosition(0, Console.CursorTop + 1);
                        }

                        Console.SetCursorPosition(ConsoleLeft, ConsoleTop);

                        int pair = 0;
                        foreach(var cmd in HTTPServerCommands.Commands)
                        {
                            if (cmd.ToLower().Contains(command.ToLower()))
                            {
                                pair++;
                            }
                        }
                        if(pair==1)
                        {
                            foreach (var cmd in HTTPServerCommands.Commands)
                            {
                                if (cmd.ToLower().Contains(command.ToLower()))
                                {
                                    command = cmd;
                                    ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                                        new ConsoleTextColor(command, ConsoleColor.Green));

                                    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                    continue;
                                }
                            }
                        }
                        if (pair > 1)
                        {
                            foreach (var cmd in HTTPServerCommands.Commands)
                            {
                                if (cmd.ToLower().Contains(command.ToLower()))
                                {
    

                                    Console.SetCursorPosition(3, Console.CursorTop + 1);

                                    ConsoleText.Write(cmd, ConsoleColor.DarkGreen);

                                    Console.SetCursorPosition(Console.CursorLeft + 3, Console.CursorTop);

                                    continue;
                                }
                            }
                            Console.SetCursorPosition(ConsoleLeft, ConsoleTop);
                        }

                        continue;
                    }

                    if (key.Key == ConsoleKey.Backspace)
                    {
                        Console.Write(" ");
                        Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

                        if (Console.CursorLeft < ConsoleTextSettings.UsNamPref.Length)
                        {
                            ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor));
                            continue;
                        }

                        string temp = "";
                        for (int i = 0; i < command.Length; i++)
                        {
                            if (i != command.Length - 1 + offset)
                                temp += command[i];
                        }
                        command = temp;
                        ConsoleText.WriteFromStart(new ConsoleTextColor("                                                                                                                                                           ", ConsoleColor.White));
                        Console.SetCursorPosition(ConsoleLeft, ConsoleTop);
                        ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor), new ConsoleTextColor(command, ConsoleColor.White));
                        Console.SetCursorPosition(ConsoleLeft - 1, ConsoleTop);

                        if (HTTPServerCommands.Commands.Contains(command.Split(" ")[0]))
                        {
                            if (command.Split(" ").Length > 1)
                            {
                                string args = " ";
                                for (int i = 1; i < command.Split(" ").Length; i++)
                                {
                                    if (i == command.Split(" ").Length - 1) args += command.Split(" ")[i];
                                    else args += command.Split(" ")[i] + " ";
                                }
                                ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                                    new ConsoleTextColor(command.Split(" ")[0], ConsoleColor.Green),
                                    new ConsoleTextColor(args, ConsoleColor.Cyan));

                            }
                            else
                            {
                                ConsoleText.WriteFromStart(new ConsoleTextColor(ConsoleTextSettings.UsNamPref, ConsoleTextSettings.UserColor),
                                    new ConsoleTextColor(command.Split(" ")[0], ConsoleColor.Green));
                            }
                        }

                        continue;
                    }


                    string t = "";

                    if (offset < 0)
                    {
                        for (int i = 0; i < command.Length + offset; i++)
                        {
                            t += command[i];
                        }
                        t += key.KeyChar;
                        for(int i= command.Length + offset;i< command.Length;i++)
                        {
                            t += command[i];
                        }
                        command = t;
                        ConsoleText.WriteFromStart(new ConsoleTextColor("Admin#", ConsoleColor.Blue), new ConsoleTextColor(command, ConsoleColor.White));
                        Console.SetCursorPosition(ConsoleLeft + 1, ConsoleTop);
                        goto check_for_commands;
                    }


                    command += key.KeyChar;

                    check_for_commands:
                    if (HTTPServerCommands.Commands.Contains(command.Split(" ")[0]))
                    {
                        if (command.Split(" ").Length > 1)
                        {
                            string args = " ";
                            for (int i = 1; i < command.Split(" ").Length; i++)
                            {
                                if (i == command.Split(" ").Length - 1) args += command.Split(" ")[i];
                                else args += command.Split(" ")[i] + " ";
                            }
                            ConsoleText.WriteFromStart(new ConsoleTextColor("Admin#", ConsoleColor.Blue),
                                new ConsoleTextColor(command.Split(" ")[0], ConsoleColor.Green),
                                new ConsoleTextColor(args, ConsoleColor.Cyan));

                        }
                        else
                        {
                            ConsoleText.WriteFromStart(new ConsoleTextColor("Admin#", ConsoleColor.Blue),
                                new ConsoleTextColor(command.Split(" ")[0], ConsoleColor.Green));
                        }
                    }
                }*/