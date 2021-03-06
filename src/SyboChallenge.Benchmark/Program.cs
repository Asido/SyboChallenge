﻿using Grace.DependencyInjection;
using Grace.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SyboChallenge.Core;
using SyboChallenge.Core.Setup;
using SyboChallenge.Module.User;
using SyboChallenge.Module.User.Abstraction;
using System;
using System.IO;

namespace SyboChallenge.Benchmark
{
    class Program
    {
        public static AppSettings AppSettings { get; private set; }
        public static IServiceProvider RootServiceProvider { get; private set; }

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();
            AppSettings = AppSettings.From(config);

            var services = new ServiceCollection();
            services.AddSingleton(AppSettings);
            services.AddInstaller();
            services.AddUser(builder => builder.UseAzureTableStorage(options => options.ConnectionString = AppSettings.ConnectionStrings.User));

            RootServiceProvider = new DependencyInjectionContainer().Populate(services);

            using (var scope = RootServiceProvider.CreateScope())
            {
                var installer = scope.ServiceProvider.GetRequiredService<Installer>();
                installer.Execute().Wait();
            }

            Start();
        }

        private static void Start()
        {
            using (var scope = RootServiceProvider.CreateScope())
            {
                var benchmark = new Benchmark(scope.ServiceProvider.GetRequiredService<IUserService>());
                benchmark.Start().Wait();
            }
        }
    }
}
