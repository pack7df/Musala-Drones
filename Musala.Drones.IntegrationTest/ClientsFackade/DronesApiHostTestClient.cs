using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Diagnostics;

namespace Musala.Drones.IntegrationTest.ClientsFackade
{
    public class DronesApiHostTestClient
    {
        public class TestStartupFilter : IStartupFilter
        {
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    builder.UseMiddleware<FakeTestMiddleware>();
                    next(builder);
                };
            }
        }

        public class FakeTestMiddleware
        {
            private readonly RequestDelegate next;
            private readonly IPAddress fakeIpAddress = IPAddress.Loopback;
            public FakeTestMiddleware(RequestDelegate next)
            {
                this.next = next;
            }

            public async Task Invoke(HttpContext httpContext)
            {
                httpContext.Connection.RemoteIpAddress = fakeIpAddress;

                await this.next(httpContext);
            }
        }

        public DronesApiHostTestClient()
        {
            var dir = Directory.GetCurrentDirectory();
            builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddTransient<IStartupFilter, TestStartupFilter>();
                })
                .UseTestServer()
                .UseEnvironment("test")
                .UseKestrel()
                .UseContentRoot(dir)
                .UseConfiguration(
                        new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .SetBasePath(dir)
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile("appsettings.test.json")
                        .Build())
                .ConfigureLogging((logBuilder) =>
                {
                    logBuilder.ClearProviders(); // removes all providers from LoggerFactory
                    logBuilder.AddConsole();
                    logBuilder.AddTraceSource("Information, ActivityTracing"); // Add Trace listener provider
                });
        }

        private TestServer server;

        public HttpClient HttpClient { get; private set; }

        public IServiceProvider ServicesProviders { get; private set; }

        private IWebHostBuilder builder = null;

        public DronesApiHostTestClient OverwriteConfig(Action<IServiceCollection> config)
        {
            builder = builder.ConfigureTestServices(config);
            return this;
        }

        private bool clearDb = false;

        public DronesApiHostTestClient ClearDb()
        {
            this.clearDb = true;
            return this;
        }

        public DronesApiHostTestClient MockDb()
        {
            builder = builder.ConfigureTestServices((sp) =>
            {

            });
            return this;
        }

        public DronesApiHostTestClient Initialize<T>() where T : class
        {
            try
            {
                builder.UseStartup<T>();
                server = new TestServer(builder);
                ServicesProviders = server.Services;
                HttpClient = server.CreateClient();
                HttpClient.Timeout = TimeSpan.FromSeconds(30);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return this;
        }
    }
}
