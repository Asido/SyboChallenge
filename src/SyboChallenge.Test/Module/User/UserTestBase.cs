using Microsoft.Extensions.DependencyInjection;
using SyboChallenge.Module.User.Abstraction;
using System;

namespace SyboChallenge.Test.Module.User
{
    public class UserTestBase : IDisposable
    {
        public readonly IUserService UserService;

        private readonly IServiceScope scope;

        public UserTestBase(UserFixture fixture)
        {
            scope = fixture.RootServiceProvider.CreateScope();
            UserService = scope.ServiceProvider.GetRequiredService<IUserService>();
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
