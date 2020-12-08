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
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = Encoding.UTF8.GetBytes("{}").LongLength;

                byte[] data = Encoding.UTF8.GetBytes(responseString);
                await resp.OutputStream.WriteAsync(data);
                resp.Close();
                
            }
        }

        public delegate void LogMessage(string message);
    }


    public class Client
    {
        public static HttpClient client = new HttpClient();
        public static Task currentConnection;
        

        public static void sendRequest(object sender)
        {
            currentConnection = getRequest(sender);
        }

        private static async Task getRequest(object sender)
        {
            var json = JsonConvert.SerializeObject(sender);
            var data = new StringContent(json, Encoding.UTF8);
            var content = await client.PostAsync("http://localhost:8000/",  data);
            GameModeContainer.Get().ProcessRequset(content);
        } 
    }
}
