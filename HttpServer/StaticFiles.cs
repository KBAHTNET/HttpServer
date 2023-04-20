using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    public static class StaticFiles
    {
        public static string HTMLSPath = "/static/htmls";
        public static string ImagesPath = "/static/img";
        public static string CSSPath = "/static/css";
        public static string JSPath = "/static/js";

        public static List<string> staticUrls = new List<string>
        {
           HTMLSPath,
           ImagesPath,
           CSSPath,
           JSPath,
           "loginstaticfiles",
           "chatstaticfiles",
           "registrationfiles",
           "favicon.ico"
        };


        public static void Init(ref Socket client, string Url)
        {
            if (Url == "/favicon.ico")
            {
                Response.MainIcon(ref client, Environment.CurrentDirectory + "/static/img/icon.svg");
            }
            try
            {
                if (Url!=null)
                {
                    if (Url.Split('/')[1] == "loginstaticfiles")
                    {
                        string pathtoStaticLoginFile = "";
                        for (int i = 2; i < Url.Split('/').Length; i++)
                        {
                            pathtoStaticLoginFile += "/" + Url.Split('/')[i];
                            
                        }

                        Console.WriteLine(pathtoStaticLoginFile);
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length-1].ToLower() == "js")
                        {
                            byte[] javascript = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = javascript;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.js;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "jpeg" || pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "jpg")
                        {
                            byte[] jpeg = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = jpeg;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.jpeg;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "gif")
                        {
                            byte[] gif = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = gif;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.gif;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "png")
                        {
                            byte[] png = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = png;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.png;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "ico")
                        {
                            byte[] ico = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = ico;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.ico;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "svg")
                        {
                            byte[] svg = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = svg;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.svg;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "css")
                        {
                            byte[] css = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = css;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.css;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('/')[1] == "fonts" && pathtoStaticLoginFile.Contains("ttf"))
                        {
                            byte[] ttf = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login/" + pathtoStaticLoginFile.Split('?')[0]);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = ttf;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.ttf;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('/')[1] == "fonts" && pathtoStaticLoginFile.Contains("woff2"))
                        {
                            byte[] woff2 = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/login/" + pathtoStaticLoginFile.Split('?')[0]);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = woff2;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.woff2;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }

                    }

                    if(Url.Split('/')[1] == "registrationfiles")
                    {
                        string filename = Url.Split('/')[Url.Split('/').Length - 1];

                        string JSStaticRegFilesPath = "/static/htmls/registration/js/";
                        string CSSStaticRegFiles = "/static/htmls/registration/css/";
                        string ImgGifPngReFiles = "/static/htmls/registration/assets/";

                        string ext = filename.Split('.')[filename.Split('.').Length - 1];

                        HttpResponseHeader responseHeader = new HttpResponseHeader();
                        responseHeader.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                        responseHeader.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                        responseHeader.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                        if (ext == "js")
                        {
                            responseHeader.Data = File.ReadAllBytes(Environment.CurrentDirectory + JSStaticRegFilesPath + filename);
                            responseHeader.HttpContentType = HttpResponseHeader.ContentType.js;
                        }

                        if (ext == "css")
                        {
                            responseHeader.Data = File.ReadAllBytes(Environment.CurrentDirectory + CSSStaticRegFiles + filename);
                            responseHeader.HttpContentType = HttpResponseHeader.ContentType.css;
                        }

                        if (ext == "gif")
                        {
                            responseHeader.Data = File.ReadAllBytes(Environment.CurrentDirectory + ImgGifPngReFiles + filename);
                            responseHeader.HttpContentType = HttpResponseHeader.ContentType.gif;
                        }

                        if (ext == "jpg" || ext == "jpeg")
                        {
                            responseHeader.Data = File.ReadAllBytes(Environment.CurrentDirectory + ImgGifPngReFiles + filename);
                            responseHeader.HttpContentType = HttpResponseHeader.ContentType.jpeg;
                        }

                        if (ext == "png")
                        {
                            responseHeader.Data = File.ReadAllBytes(Environment.CurrentDirectory + ImgGifPngReFiles + filename);
                            responseHeader.HttpContentType = HttpResponseHeader.ContentType.png;
                        }



                        client.Send(responseHeader.CreateInBytes());
                    }

                    if (Url.Split('/')[1] == "chatstaticfiles")
                    {
                        string pathtoStaticLoginFile = "";
                        for (int i = 2; i < Url.Split('/').Length; i++)
                        {
                            pathtoStaticLoginFile += "/" + Url.Split('/')[i];

                        }

                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "js")
                        {
                            byte[] javascript = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/chat" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = javascript;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.js;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "jpeg" || pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "jpg")
                        {
                            byte[] jpeg = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/chat" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = jpeg;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.jpeg;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "gif")
                        {
                            byte[] gif = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/chat" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = gif;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.gif;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "png")
                        {
                            byte[] png = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/chat" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = png;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.png;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "ico")
                        {
                            byte[] ico = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/chat" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = ico;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.ico;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }
                        if (pathtoStaticLoginFile.Split('.')[pathtoStaticLoginFile.Split('.').Length - 1].ToLower() == "css")
                        {
                            byte[] css = File.ReadAllBytes(Environment.CurrentDirectory + "/static/htmls/chat" + pathtoStaticLoginFile);

                            HttpResponseHeader responseHeader2 = new HttpResponseHeader();
                            responseHeader2.Data = css;
                            responseHeader2.HttpConnectionType = HttpResponseHeader.ConnectionType.close;
                            responseHeader2.HttpContentType = HttpResponseHeader.ContentType.css;
                            responseHeader2.Status = HttpResponseHeader.HttpResponseStatus.OK_200;
                            responseHeader2.Version = HttpResponseHeader.HttpVersion.HTTP_1_1;

                            client.Send(responseHeader2.CreateInBytes());
                        }

                    }

                    string FilterUrl = "/" + Url.Split('/')[1] + "/" + Url.Split('/')[2];

                    if (FilterUrl.Contains(ImagesPath))
                    {
                        Console.WriteLine(Url);
                        Response.ResponseStaticIMG(ref client, Url.Split('/')[Url.Split('/').Length - 1]);

                    }
                    if (FilterUrl.Contains(CSSPath))
                    {
                        Console.WriteLine(Url);
                        Response.ResponseStaticCSS(ref client, Url.Split('/')[Url.Split('/').Length - 1]);

                    }
                    if (FilterUrl.Contains(JSPath))
                    {
                        Console.WriteLine(Url);
                        Response.ResponseStaticJS(ref client, Url.Split('/')[Url.Split('/').Length - 1]);

                    }
                }
            }
            catch { }
        }
    }
}
