using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace web_openvpn
{
    internal class Program
    {
        static async Task Main(string[] _)
        {
            int port = int.TryParse(Environment.GetEnvironmentVariable("WEB_OPENVPN_PORT"), out int _port) ? _port : 8080;
            WebServer webServer = new WebServer("+", port);

            webServer.AddStaticFile("index.html", GetResourceFile("web_openvpn.webroot.index.html"));
            webServer.AddStaticFile("bootstrap.min.css", GetResourceFile("web_openvpn.webroot.bootstrap.min.css"));
            webServer.AddStaticFile("vue.js", GetResourceFile("web_openvpn.webroot.vue.js"));

            webServer.AddHandler("clients", "GET", ClientGet);
            webServer.AddHandler("clients", "POST", ClientPost);
            webServer.AddHandler("clients", "DELETE", ClientDelete);

            webServer.AddAuthPass(Environment.GetEnvironmentVariable("WEB_OPENVPN_PASSWORD") ?? "password");

            await webServer.Loop();
        }

        private static async Task<(string, HttpStatusCode)> ClientGet(string[] path, string query)
        {
            if (path.Length > 1)
            {
                string name = path[1];
                var clientFile = ClientCommands.GetClientFile(name);
                return (clientFile, HttpStatusCode.OK);
            }
            else
            {
                var clients = await ClientCommands.ListOfClients();
                string result = string.Join(",", clients);
                return (result, HttpStatusCode.OK);
            }
        }

        private static async Task<(string, HttpStatusCode)> ClientPost(string[] path, string content)
        {
            if (await ClientCommands.CreateClient(content)) return ("OK", HttpStatusCode.OK);
            else return ("Error", HttpStatusCode.BadRequest);
        }

        private static async Task<(string, HttpStatusCode)> ClientDelete(string[] path, string query)
        {
            if (path.Length > 1 && await ClientCommands.RemoveClient(path[1])) return ("OK", HttpStatusCode.OK);
            return ("Error", HttpStatusCode.BadRequest);
        }

        private static string GetResourceFile(string resourceName)
        {
            using Stream stream = Assembly.GetAssembly(typeof(Program)).GetManifestResourceStream(resourceName);
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
