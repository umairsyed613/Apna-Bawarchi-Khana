using System;
using System.Diagnostics;

using ApnaBawarchiKhana.Server.Helper;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

namespace ApnaBawarchiKhana.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Information()
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                            .Enrich.FromLogContext()
                            .WriteTo.Sink(new InMemorySink())
                            //.WriteTo.File(
                            //                @"ApnaBawarchiKhanaLogs.txt",
                            //                fileSizeLimitBytes: 1_000_000,
                            //                rollOnFileSizeLimit: true,
                            //                shared: true,
                            //                flushToDiskInterval: TimeSpan.FromSeconds(10))
                            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                            .CreateLogger();

            try
            {
                CreateHostBuilder(args).UseSerilog().Build().Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseWebRoot("wwwroot");
                    webBuilder.UseStaticWebAssets();
                });
    }
}
