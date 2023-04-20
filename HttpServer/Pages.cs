using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace HttpServer
{
    public static class Pages
    {
        public static void Init(ref Socket client, HttpRequestHeader request)
        {
            try
            {
                if (request.Url == "/")
                {
                    Response.LoginPage(ref client);
                }
                else if (request.Url == "/login")
                {
                    Response.LoginPage(ref client);
                }
                else if (request.Url == "/registration")
                {
                    Response.RegistrationPage(ref client);
                }
                else if (request.Url.Contains("/registration_end")) 
                {
                    Response.UseerRegistration(ref client, request);
                }
                else if (!StaticFiles.staticUrls.Contains(request.Url.Split('/')[1]))
                {
                    Console.WriteLine("URL -------" + request.Url);
                    Console.WriteLine(request.Url.Split('/')[1]);
                    Response.Page404(ref client, request.Url);
                }
            }
            catch { }
        }
    }
}
