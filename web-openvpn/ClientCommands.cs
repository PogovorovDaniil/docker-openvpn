using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace web_openvpn
{
    public class ClientCommands
    {
        private static async Task<(string output, int exitCode)> Exec(string program, params string[] args)
        {
            Process process = new Process();
            process.StartInfo.FileName = program;
            foreach (string arg in args)
            {
                process.StartInfo.ArgumentList.Add(arg);
            }
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            string text = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();
            int exitCode = process.ExitCode;

            return (text, exitCode);
        }

        public static async Task<bool> CreateClient(string clientName)
        {
            var result = await Exec("./client-create", clientName);
            Console.WriteLine(result);
            return result.exitCode == 0;
        }

        public static async Task<bool> RemoveClient(string clientName)
        {
            var result = await Exec("./client-remove", clientName);
            Console.WriteLine(result);
            return result.exitCode == 0;
        }

        public static async Task<string[]> ListOfClients()
        {
            var result = await Exec("./client-list");
            Console.WriteLine(result);
            return result.output.Replace("\r", "").TrimEnd('\n').Split("\n");
        }

        public static string GetClientFile(string name)
        {
            try
            {
                using FileStream fileStream = new FileStream($"/etc/openvpn/client/{name}.ovpn", FileMode.Open);
                using StreamReader reader = new StreamReader(fileStream);
                return reader.ReadToEnd();
            }
            catch (FileNotFoundException)
            {
                return "";
            }
        }
    }
}
