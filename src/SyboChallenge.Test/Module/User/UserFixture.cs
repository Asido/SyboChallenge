using Microsoft.Extensions.DependencyInjection;
using SyboChallenge.Module.User;
using Xunit;

namespace SyboChallenge.Test.Module.User
{
    public class UserFixture : ModuleFixture
    {
        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddUser(builder => builder.UseAzureTableStorage(options =>
                options.ConnectionString = AppSettings.ConnectionStrings.User));
        }

        protected override void Configure() { }
    }

    [CollectionDefinition(Name)]
    public class UserTestCollection : ICollectionFixture<UserFixture>
    {
        public const string Name = "User test collection";
    }
}
