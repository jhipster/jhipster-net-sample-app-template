using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using JHipsterNet.Logging;
using ILogger = Serilog.ILogger;

namespace JHipsterNetSampleApplication {
    public class Program {

        public static int Main(string[] args)
        {
            try {

                ConfigureLogger();

                Log.Information($"Starting web host");
                CreateWebHostBuilder(args).Build().Run();
                return 0;

            }
            catch (Exception ex) {

                Log.Fatal(ex, $"Host terminated unexpectedly");
                return 1;

            }
            finally {

                Log.CloseAndFlush();

            }


        }

        public static IWebHostBuilder CreateWebHostBuilder(params string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
        }

        /// <summary>
        /// Gets the current application configuration
        /// from global and specific appsettings.
        /// </summary>
        /// <returns>Return the current <see cref="IConfiguration"/></returns>
        private static IConfiguration GetAppConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment}.json", true)
                .Build();
        }

        /// <summary>
        /// Configure application logger
        /// </summary>
        /// <returns></returns>
        private static ILogger ConfigureLogger()
        {
            var appConfiguration = GetAppConfiguration();

            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.With<LoggerNameEnricher>()
                .ReadFrom.Configuration(appConfiguration);

            return Log.Logger = loggerConfiguration.CreateLogger();
        }
    }
}
