using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using Newtonsoft.Json;

namespace Controller
{
    public class HttpServer
    {
        public static HttpListener listener;
        public static LogMessage logMessage;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";


        public static async Task HadlerIncominfConnections()
        {
            bool runServer = true;

            while(runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                //logMessage("Request");
                //logMessage(req.HttpMethod);

                var requestBody = ctx.Request.InputStream.ToString();
                var content = JsonConvert.DeserializeObject<object>(requestBody, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                var response = GameModeContainer.Get().ProcessRequset(content);
                var responseString = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                byte[] data = Encoding.UTF8.GetBytes(responseString);
                await resp.OutputStream.WriteAsync(data);

            }
        }

        public delegate void LogMessage(string message);
    }


    public class Client
    {
        public static HttpClient client = new HttpClient();

        public static async Task<object> sendRequest(object sender)
        {
            HttpResponseMessage response = await client.GetAsync("http://localhost:8000/");
            string responseBody = await response.Content.ReadAsStringAsync();
            var content = JsonConvert.DeserializeObject<object>(responseBody, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

            return content;
        }

    }


}
