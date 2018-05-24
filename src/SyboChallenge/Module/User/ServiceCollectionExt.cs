using Microsoft.Extensions.DependencyInjection;
using SyboChallenge.Core;
using SyboChallenge.Core.AzureTableStorage;
using SyboChallenge.Module.User.Abstraction;
using SyboChallenge.Module.User.Service;
using SyboChallenge.Module.User.Store;
using SyboChallenge.Module.User.Store.Setup;
using System;

namespace SyboChallenge.Module.User
{
    public class UserBuilder
    {
        public IServiceCollection Services { get; }

        public UserBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public UserBuilder UseAzureTableStorage(Action<AzureTableOptions> configureOptions)
        {
            Services.AddSingleton<AzureTableProvider>();
            Services.AddStore<IUserStore, UserStore>();
            Services.AddInstallerStep<SchemaInstaller>();
            Services.AddSingleton(provider => {
                var options = new AzureTableOptions();
                configureOptions(options);
                return options;
            });

            return this;
        }
    }

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddUser(this IServiceCollection services, Action<UserBuilder> configureAction)
        {
            services
                .AddService<IUserService, UserService>();

            var builder = new UserBuilder(services);
            configureAction(builder);

            return services;
        }
    }
}
