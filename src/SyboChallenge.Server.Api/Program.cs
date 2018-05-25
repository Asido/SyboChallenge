using Grace.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SyboChallenge.Core.Setup;

namespace SyboChallenge.Server.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var installer = scope.ServiceProvider.GetRequiredService<Installer>();
                installer.Execute().Wait();
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseGrace()
                .UseApplicationInsights()
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((host, config) =>
                {
                    config.AddJsonFile("AppSettings.json", optional: false);
                    config.AddJsonFile($"AppSettings.{host.HostingEnvironment.EnvironmentName}.json", optional: false);
                    config.AddJsonFile("AppSettings.User.json", optional: true);
                    config.AddEnvironmentVariables();
                })
                .Build();
    }
}
