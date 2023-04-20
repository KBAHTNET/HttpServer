 using System;

namespace HttpServer
{
    class Program
    {
        public static int Port = 80;
        static void Main(string[] args)
        {
            Console.Title = "HTTPServer";
            goto main;
        other: 
            args = null;
        main:
            if (args != null)
            {
                try
                {
                    Port = Convert.ToInt32(args[0]);
                    if (Port > 65535) goto other;
                    Console.WriteLine("Приложение будет запущено на " + Port + " порту");
                }
                catch
                {
                    goto other;
                }
            }
            else
            {
            start:
                Console.Write("Введите порт, на котором будет запущен сервер: ");
                try
                {
                    Port = Convert.ToInt32(Console.ReadLine());
                    if (Port > 65535) goto start;
                }
                catch
                {
                    Console.WriteLine("Неверный аргумент");
                    goto start;
                }
            }
            HTTPServer server = new HTTPServer(Port);
            server.Start();
        }
    }
}
