using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Net.Http;
using Newtonsoft.Json;
using System.IO;

namespace Controller
{
    public class HttpServer
    {
        public static HttpListener listener;
        public static LogMessage logMessage;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;



        public static async Task HandlerIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                //logMessage("Request");
                //logMessage(req.HttpMethod);
                var serializer = new JsonSerializer();
                object content;
                using (var sr = new StreamReader(ctx.Request.InputStream))
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    content = serializer.Deserialize(jsonTextReader);
                }


                var response = GameModeContainer.Get().ProcessRequset(content);
                var responseString = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                byte[] data = Encoding.UTF8.GetBytes(responseString);
                await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("{}"));
                resp.OutputStream.Close();
                resp.Close();
                
            }
        }

        public delegate void LogMessage(string message);
    }


    public class Client
    {
        public static HttpClient client = new HttpClient();

        public static async Task<object> sendRequest(object sender)
        {
            var response = await client.GetStreamAsync("http://localhost:8000/");
            object content;
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(response))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                content = serializer.Deserialize(jsonTextReader);
            }

            return content;
        }

    }


}
