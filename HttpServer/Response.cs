using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using HttpServer.DatabaseSession;

namespace HttpServer
{
    class Response
    {
        public static void LoginPage(ref Socket client)
        {
            string html = File.ReadAllText(Environment.CurrentDirectory + "/static/htmls/login/index.html" );

            HttpResponseHeader responseHeader = new HttpResponseHeader();
            responseHeader.Data = Encoding.UTF8.GetBytes(html);
            responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
            responseHeader.HttpContentType = HttpResponseHeader.ContentType.html;
            responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
            responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

            //Console.WriteLine("RESPONSE:\n_____________________________\n" + responseHeader.Create() + "\n_____________________________\n");
            client.Send(responseHeader.CreateInBytes());
        }

        public static void RegistrationPage(ref Socket client)
        {
            string html = File.ReadAllText(Environment.CurrentDirectory + "/static/htmls/registration/index.html");

            HttpResponseHeader responseHeader = new HttpResponseHeader();
            responseHeader.Data = Encoding.UTF8.GetBytes(html);
            responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
            responseHeader.HttpContentType = HttpResponseHeader.ContentType.html;
            responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
            responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;
            client.Send(responseHeader.CreateInBytes());
        }

        public static void UseerRegistration(ref Socket client, HttpRequestHeader request)
        {
            ConsoleText.ReWrite(request.Arguments["nickname"], "Admin#",ConsoleColor.Red, ConsoleColor.Blue);
            ConsoleText.ReWrite(request.Arguments["surname"], "Admin#", ConsoleColor.Red, ConsoleColor.Blue);
            ConsoleText.ReWrite(request.Arguments["name"], "Admin#", ConsoleColor.Red, ConsoleColor.Blue);
            ConsoleText.ReWrite(request.Arguments["patronymic"], "Admin#", ConsoleColor.Red, ConsoleColor.Blue);
            ConsoleText.ReWrite(request.Arguments["birth"], "Admin#", ConsoleColor.Red, ConsoleColor.Blue);
            ConsoleText.ReWrite(request.Arguments["country"], "Admin#", ConsoleColor.Red, ConsoleColor.Blue);
            ConsoleText.ReWrite(request.Arguments["password_sha512"], "Admin#", ConsoleColor.Red, ConsoleColor.Blue);
            ConsoleText.ReWrite(request.Arguments["avatar"], "Admin#", ConsoleColor.Red, ConsoleColor.Blue);
            string user_registration = $"INSERT INTO users (nickname,password_sha512,avatar,surname,name,patronymic,registration_date,country)VALUES" +
                $"(`{request.Arguments["nickname"]}`,`{request.Arguments["password_sha512"]}`,`{request.Arguments["avatar"]}`,`{request.Arguments["surname"]}`," +
                $"`{request.Arguments["name"]}`,`{request.Arguments["patronymic"]}`,`{DateTime.Now.ToShortDateString()}`,`{request.Arguments["country"]}`);";
            HTTPServer.sqlServer.Send(System.Text.Encoding.UTF8.GetBytes(user_registration));
            //Console.WriteLine(user_registration);
        }

        //public static void RegistrationFiles(ref Socket client, string filename)
        //{
        //    string JSStaticRegFilesPath = "/static/htmls/registration/js/";
        //    string CSSStaticRegFiles = "/static/htmls/registration/css/";
        //    string ImgGifPngReFiles = "/static/htmls/registration/assets/";

        //    string ext = filename.Split('.')[filename.Split('.').Length - 1];

        //    HttpResponseHeader responseHeader = new HttpResponseHeader();
        //    responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
        //    responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
        //    responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

        //    if (ext == "js")
        //    {
        //        responseHeader.Data = File.ReadAllBytes(JSStaticRegFilesPath + filename);
        //        responseHeader.HttpContentType = HttpResponseHeader.ContentType.js;
        //    }

        //    if (ext == "css")
        //    {
        //        responseHeader.Data = File.ReadAllBytes(CSSStaticRegFiles + filename);
        //        responseHeader.HttpContentType = HttpResponseHeader.ContentType.css;
        //        Console.WriteLine(Encoding.UTF8.GetString(responseHeader.Data));

        //        Console.WriteLine("================================================================");
        //    }

        //    if (ext == "gif")
        //    {
        //        responseHeader.Data = File.ReadAllBytes(ImgGifPngReFiles + filename);
        //        responseHeader.HttpContentType = HttpResponseHeader.ContentType.gif;
        //    }

        //    if (ext == "jpg" || ext=="jpeg")
        //    {
        //        responseHeader.Data = File.ReadAllBytes(ImgGifPngReFiles + filename);
        //        responseHeader.HttpContentType = HttpResponseHeader.ContentType.jpeg;
        //    }

        //    if (ext == "png")
        //    {
        //        responseHeader.Data = File.ReadAllBytes(ImgGifPngReFiles + filename);
        //        responseHeader.HttpContentType = HttpResponseHeader.ContentType.png;
        //    }



        //    client.Send(responseHeader.CreateInBytes());
        //}

        public static void Page404(ref Socket client, string Url)
        {
            if (Url == "/page404/style.css")
            {
                string css = File.ReadAllText(Environment.CurrentDirectory + "/static/htmls/page404/style.css");

                HttpResponseHeader responseHeader = new HttpResponseHeader();
                responseHeader.Data = Encoding.UTF8.GetBytes(css);
                responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                responseHeader.HttpContentType = HttpResponseHeader.ContentType.css;
                responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                client.Send(responseHeader.CreateInBytes());
                responseHeader.Data = new byte[1];
                Console.WriteLine(responseHeader.Create());
            }
            else
            {
                if (Url == "/404")
                {
                    string html = File.ReadAllText(Environment.CurrentDirectory + "/static/htmls/page404/index2.html");

                    HttpResponseHeader responseHeader = new HttpResponseHeader();
                    responseHeader.Data = Encoding.UTF8.GetBytes(html);
                    responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                    responseHeader.HttpContentType = HttpResponseHeader.ContentType.html;
                    responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                    responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                    client.Send(responseHeader.CreateInBytes());
                }
                else
                {
                    string html = File.ReadAllText(Environment.CurrentDirectory + "/static/htmls/page404/index1.html");

                    HttpResponseHeader responseHeader = new HttpResponseHeader();
                    responseHeader.Data = Encoding.UTF8.GetBytes(html);
                    responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                    responseHeader.HttpContentType = HttpResponseHeader.ContentType.html;
                    responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                    responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                    client.Send(responseHeader.CreateInBytes());
                }
            }
        }

        #region StaticFiles
        public static void ResponseStaticJS(ref Socket client, string filename)
        {
            byte[] data = File.ReadAllBytes(Environment.CurrentDirectory + StaticFiles.JSPath + "/" + filename);

            HttpResponseHeader responseHeader = new HttpResponseHeader();
            responseHeader.Data = data;
            responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
            responseHeader.HttpContentType = HttpResponseHeader.ContentType.js;
            responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
            responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;
            client.Send(responseHeader.CreateInBytes());
        }

        public static void ResponseStaticCSS(ref Socket client, string filename)
        {
            byte[] data = File.ReadAllBytes(Environment.CurrentDirectory + StaticFiles.CSSPath + "/" + filename);

            HttpResponseHeader responseHeader = new HttpResponseHeader();
            responseHeader.Data = data;
            responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
            responseHeader.HttpContentType = HttpResponseHeader.ContentType.css;
            responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
            responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;
            client.Send(responseHeader.CreateInBytes());
        }

        public static void ResponseStaticIMG(ref Socket client, string filename)
        {

            byte[] data = File.ReadAllBytes(Environment.CurrentDirectory + StaticFiles.ImagesPath + "/" + filename);
            string s = "";

            /*Console.WriteLine("Читаем " + Environment.CurrentDirectory + StaticFiles.ImagesPath + "/" + filename);
            for (int i = 0; i < 100; i++) s += Convert.ToString(data[i], 16);
            Console.WriteLine(s);*/

            HttpResponseHeader responseHeader = new HttpResponseHeader();
            //responseHeader.Data = data;
            responseHeader.SetData(ref data);
            responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;

            /*try
            {*/
                if (filename.Split('.')[1].ToLower() == "jpeg" || filename.Split('.')[1].ToLower() == "jpg") responseHeader.HttpContentType = HttpResponseHeader.ContentType.jpeg;
                if (filename.Split('.')[1].ToLower() == "png") responseHeader.HttpContentType = HttpResponseHeader.ContentType.png;
                if (filename.Split('.')[1].ToLower() == "gif") responseHeader.HttpContentType = HttpResponseHeader.ContentType.gif;
           /* }
            catch 
            {
                responseHeader.HttpContentType = HttpResponseHeader.ContentType.jpeg;
            }
           */
            responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
            responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

            client.Send(responseHeader.CreateInBytes());
            responseHeader.Data = new byte[0];
            Console.WriteLine("RESPONSE:\n_____________________________\n" + responseHeader.Create() + "\n_____________________________\n");
            //try
            //{
            //    byte[] data = File.ReadAllBytes(Environment.CurrentDirectory + "/static/img/" + filename);
            //    string imageType = filename.Split('.')[1];
            //    string Status = "200 OK";
            //    string ResponseHeader = "";
            //    if (imageType == "jpeg" || imageType == "jpg")
            //    {
            //        ResponseHeader = String.Format(
            //           "{0} {1}\nServer:{2}\nContent-Language:en\n" +
            //           "Content-type:image/jpeg\nAccept-Ranges:bytes\nContentLength:{3}\nConnection:close\n\n",
            //           HTTPServer.VERSION, Status, HTTPServer.SERVERNAME, data.Length);
            //    }
            //    if (imageType == "png")
            //    {
            //        ResponseHeader = String.Format(
            //           "{0} {1}\nServer:{2}\nContent-Language:en\n" +
            //           "Content-type:image/png\nAccept-Ranges:bytes\nContentLength:{3}\nConnection:close\n\n",
            //           HTTPServer.VERSION, Status, HTTPServer.SERVERNAME, data.Length);
            //    }
            //    if (imageType == "gif")
            //    {
            //        ResponseHeader = String.Format(
            //           "{0} {1}\nServer:{2}\nContent-Language:en\n" +
            //           "Content-type:image/gif\nAccept-Ranges:bytes\nContentLength:{3}\nConnection:close\n\n",
            //           HTTPServer.VERSION, Status, HTTPServer.SERVERNAME, data.Length);
            //    }

            //    byte[] rh = Encoding.UTF8.GetBytes(ResponseHeader);
            //    byte[] d = new byte[rh.Length + data.Length];

            //    for (int i = 0; i < rh.Length; i++)
            //    {
            //        d[i] = rh[i];
            //    }
            //    for (int i = rh.Length; i < rh.Length + data.Length; i++)
            //    {
            //        d[i] = data[i - rh.Length];
            //    }

            //    client.Send(d);
            //}
            //catch
            //{
            //    Console.WriteLine("Неверный путь к изображению");
            //}
        }
        #endregion

        public static void MainIcon(ref Socket client, string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                string Status = "200 OK";
                //string ResponseHeader = String.Format(
                //"{0} {1}\nServer:{2}\nContent-Language:en\n" +
                //"Content-type:image/vnd.microsoft.icon\nAccept-Ranges:bytes\nContentLength:{3}\nConnection:close\n\n",
                //HTTPServer.VERSION, Status, HTTPServer.SERVERNAME, data.Length);

                string ResponseHeader = String.Format(
                    "{0} {1}\nServer:{2}\nContent-Language:en\n" +
                    "Content-type:image/svg+xml\nAccept-Ranges:bytes\nContentLength:{3}\nConnection:close\n\n",
                    HTTPServer.VERSION, Status, HTTPServer.SERVERNAME, data.Length);

                byte[] rh = Encoding.UTF8.GetBytes(ResponseHeader);
                byte[] d = new byte[rh.Length + data.Length];

                for (int i = 0; i < rh.Length; i++)
                {
                    d[i] = rh[i];
                }
                for (int i = rh.Length; i < rh.Length + data.Length; i++)
                {
                    d[i] = data[i - rh.Length];
                }

                client.Send(d);
            }
            catch 
            {
                Console.WriteLine("Неверный путь к иконке или неверный формат у иконки");
            }
        }



        private string Img2Base64(string Path)
        {
            byte[] imageArray = System.IO.File.ReadAllBytes(Path);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            return "data:image/gif;base64," + base64ImageRepresentation;
        }

        public string TextToHtml(string text)
        {
            text = HttpUtility.HtmlEncode(text);
            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "<br>\r\n");
            text = text.Replace("  ", " &nbsp;");
            return text;
        }
    }
}
