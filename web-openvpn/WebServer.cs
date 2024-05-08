using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace web_openvpn
{
    public class WebServer
    {
        private Dictionary<string, string> staticFiles;
        private Dictionary<(string action, string method), Func<string[], string, Task<(string content, HttpStatusCode statusCode)>>> requestHandler;
        private HttpListener server;
        private string password;
        private Guid token;

        public WebServer(string address, int port = 80)
        {
            server = new HttpListener();

            string protocol = port == 443 ? "https" : "http";
            string portText = port != 80 && port != 443 ? $":{port}" : "";
            string urlPrefix = $"{protocol}://{address}{portText}/";
            server.Prefixes.Add(urlPrefix);

            staticFiles = new Dictionary<string, string>();
            requestHandler = new Dictionary<(string action, string method), Func<string[], string, Task<(string content, HttpStatusCode statusCode)>>>();

            password = "";
            token = new Guid();
        }

        public async Task Loop()
        {
            server.Start();
            while (server.IsListening)
            {
                var context = await server.GetContextAsync();
                var request = context.Request;
                var response = context.Response;
                (string[] path, string[] query, string method) = ParseRequest(request);

                if (staticFiles.ContainsKey(path[0]))
                {
                    SendContent(context.Response, staticFiles[path[0]]);
                    continue;
                }

                string content = "";
                if (method == "POST")
                {
                    using StreamReader reader = new StreamReader(request.InputStream);
                    content = reader.ReadToEnd();
                }

                if (path[0] == "auth" && content == password)
                {
                    SendContent(context.Response, token.ToString());
                    continue;
                }

                if (request.Headers["Authorization"] != $"Bearer {token}")
                {
                    SendContent(context.Response, "Authorization failed", HttpStatusCode.Forbidden);
                    continue;
                }

                if (requestHandler.TryGetValue((path[0], method), out var handler))
                {
                    if (method == "GET" || method == "DELETE")
                    {
                        var result = await handler(path, string.Join("&", query));
                        SendContent(response, result.content, result.statusCode);
                        continue;
                    }
                    else
                    {
                        var result = await handler(path, content);
                        SendContent(response, result.content, result.statusCode);
                        continue;
                    }
                }
                SendContent(context.Response, "404 Not Found", HttpStatusCode.NotFound);
            }
        }

        public void AddAuthPass(string pass) => this.password = pass;
        public void AddHandler(string action, string method, Func<string[], string, Task<(string, HttpStatusCode)>> handler) => requestHandler[(action, method)] = handler;
        public void AddStaticFile(string path, string content) => staticFiles[path] = content;

        private (string[] path, string[] query, string method) ParseRequest(HttpListenerRequest request)
        {
            string requestUrl = request.RawUrl;
            if (requestUrl == "/") requestUrl = "/index.html";
            requestUrl = requestUrl.TrimStart('/');
            string[] requestUrlBuffer = requestUrl.Split("?");
            string[] path = requestUrlBuffer[0].Split("/");
            string[] query = requestUrlBuffer.Length > 1 ? requestUrlBuffer[1].Split("&") : Array.Empty<string>();

            Console.WriteLine($"{request.HttpMethod}: {requestUrlBuffer[0]}");

            return (path, query, request.HttpMethod);
        }
        private static async void SendContent(HttpListenerResponse response, string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            response.StatusCode = (int)statusCode;
            await response.OutputStream.WriteAsync(buffer);
            await response.OutputStream.FlushAsync();
            response.OutputStream.Close();
        }
    }
}
