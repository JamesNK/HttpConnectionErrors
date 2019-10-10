using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(kestrel =>
                    {
                        // HTTP/1.1 - No TLS
                        kestrel.ListenLocalhost(5000, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1;
                        });

                        // HTTP/1.1 - TLS
                        kestrel.ListenLocalhost(5001, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1;
                            listenOptions.UseHttps();
                        });

                        // HTTP/1.1 - Invalid TLS
                        kestrel.ListenLocalhost(5002, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1;
                            listenOptions.UseHttps("httpconnectionerrors.pfx", "password1234");
                        });

                        // HTTP/2 - No TLS
                        kestrel.ListenLocalhost(5010, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });

                        // HTTP/2 - TLS
                        kestrel.ListenLocalhost(5011, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                            listenOptions.UseHttps();
                        });

                        // HTTP/2 - Invalid TLS
                        kestrel.ListenLocalhost(5012, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                            listenOptions.UseHttps("httpconnectionerrors.pfx", "password1234");
                        });

                        // HTTP/1.1 & HTTP/2 - No TLS
                        kestrel.ListenLocalhost(5020, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });

                        // HTTP/1.1 & HTTP/2 - TLS
                        kestrel.ListenLocalhost(5021, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            listenOptions.UseHttps();
                        });

                        // HTTP/1.1 & HTTP/2 - Invalid TLS
                        kestrel.ListenLocalhost(5022, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            listenOptions.UseHttps("httpconnectionerrors.pfx", "password1234");
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
