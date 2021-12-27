using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Musala.Drones.ApiHost
{
    public static class ConfigurationUtils
    {
        public static IHostBuilder ConfigureDefaultSettings(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                       .AddJsonFile("appsettings.json", false, true)
                       .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                       .AddEnvironmentVariables();
            }).ConfigureLogging(logBuilder =>
            {
                logBuilder.ClearProviders(); // removes all providers from LoggerFactory
                logBuilder.AddConsole();
                logBuilder.AddTraceSource("Information, ActivityTracing"); // Add Trace listener provider
            });
        }
    }
}
