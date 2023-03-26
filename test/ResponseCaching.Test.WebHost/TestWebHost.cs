using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ResponseCaching.Test.WebHost;

public class TestWebHost
{
    public static bool IsTest { get; set; } = false;

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configure =>
            {
                configure.AddUserSecrets<TestWebHost>();
            })
            .ConfigureLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole(options =>
                {
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff ";
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                if (IsTest)
                {
                    webBuilder.UseTestServer();
                }
                webBuilder.UseStartup<Startup>();
            });
}
