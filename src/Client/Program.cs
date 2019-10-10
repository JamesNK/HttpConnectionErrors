using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace Client
{
    class Program
    {
        private static Dictionary<string, int> ServerPorts = new Dictionary<string, int>
        {
            ["HTTP/1.1 - No TLS"] = 5000,
            ["HTTP/1.1 - TLS"] = 5001,
            ["HTTP/1.1 - Invalid TLS"] = 5002,
            ["HTTP/2 - No TLS"] = 5010,
            ["HTTP/2 - TLS"] = 5011,
            ["HTTP/2 - Invalid TLS"] = 5012,
            ["HTTP/1.1 & HTTP/2 - No TLS"] = 5020,
            ["HTTP/1.1 & HTTP/2 - TLS"] = 5021,
            ["HTTP/1.1 & HTTP/2 - Invalid TLS"] = 5022
        };

        private static Dictionary<string, Func<int, Task>> ClientCalls = new Dictionary<string, Func<int, Task>>
        {
            ["HTTP/1.1 - No TLS"] = port => DoCall(http2: false, tls: false, port),
            ["HTTP/1.1 - TLS"] = port => DoCall(http2: false, tls: true, port),
            ["HTTP/2 - No TLS"] = port => DoCall(http2: true, tls: false, port),
            ["HTTP/2 - TLS"] = port => DoCall(http2: true, tls: true, port)
        };

        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            foreach (var clientCall in ClientCalls)
            {
                foreach (var serverPort in ServerPorts)
                {
                    await Echo(clientCall.Key, serverPort.Key);

                    await clientCall.Value(serverPort.Value);

                    Console.WriteLine();
                    Console.WriteLine("=============================");
                    Console.WriteLine();
                }
            }
        }

        private static async Task Echo(string client, string server)
        {
            var message = "Client: " + client + Environment.NewLine + "Server: " + server;

            Console.WriteLine(message);

            var httpContext = new ByteArrayContent(Encoding.UTF8.GetBytes(message));

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync($"http://localhost:5000/echo", httpContext);

            response.EnsureSuccessStatusCode();
        }

        private static async Task DoCall(bool http2, bool tls, int port)
        {
            var httpClient = new HttpClient();
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, $"{(tls ? "https" : "http")}://localhost:{port}?http2={http2}&tls={tls}");
            if (http2)
            {
                httpRequestMessage.Version = new Version(2, 0);
            }

            try
            {
                var response = await httpClient.SendAsync(httpRequestMessage);
                response.EnsureSuccessStatusCode();

                Console.WriteLine("Success!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
