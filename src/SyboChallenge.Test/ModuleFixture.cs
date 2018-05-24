using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SyboChallenge.Core;
using SyboChallenge.Core.Setup;
using System;
using System.IO;

namespace SyboChallenge.Test
{
    public abstract class ModuleFixture
    {
        public readonly AppSettings AppSettings;
        public readonly IServiceProvider RootServiceProvider;

        public ModuleFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();
            AppSettings = AppSettings.From(config);

            var services = new ServiceCollection();
            services.AddSingleton(AppSettings);
            services.AddInstaller();
            ConfigureServices(services);
            RootServiceProvider = new DependencyInjectionContainer().Populate(services);
            Configure();

            using (var scope = RootServiceProvider.CreateScope())
            {
                var installer = scope.ServiceProvider.GetRequiredService<Installer>();
                installer.Execute().Wait();
            }
        }

        protected abstract void ConfigureServices(IServiceCollection services);
        protected abstract void Configure();
    }
}
