using FluentAssertions;
using SyboChallenge.Core;
using SyboChallenge.Module.User.Abstraction;
using SyboChallenge.Test.Generator;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SyboChallenge.Test.Module.User.Service
{
    [Collection(UserTestCollection.Name)]
    public class UserServiceTest : UserTestBase
    {
        public UserServiceTest(UserFixture fixture) : base(fixture) { }

        [Fact]
        public async Task CanListUsers()
        {
            var users = await UserService.Find();
            users.Should().NotBeNull();
        }

        [Fact]
        public async Task CanCreateUser()
        {
            var name = StringGenerator.Unique();
            var user = await UserService.FindOrCreate(name);

            user.Should().NotBeNull();
            user.Id.Should().NotBe(default);
            user.Name.Should().Be(name);
        }

        [Fact]
        public async Task CanFindUnexistingUserFriendlist()
        {
            var result = await UserService.FindFriends(Guid.NewGuid());
            result.Succeeded.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCode.NotFound);
        }

        [Fact]
        public async Task CanFindExistingUserEmptyFriendlist()
        {
            var user = await UserService.FindOrCreate(StringGenerator.Unique());
            var result = await UserService.FindFriends(user.Id);
            result.Succeeded.Should().BeTrue();

            var friends = result.Value;
            friends.Should().NotBeNull();
            friends.Should().BeEmpty();
        }

        [Fact]
        public async Task CanUpdateFriendlist()
        {
            var user = await UserService.FindOrCreate(StringGenerator.Unique());
            var friendIds = new[]
            {
                (await UserService.FindOrCreate(StringGenerator.Unique())).Id,
                (await UserService.FindOrCreate(StringGenerator.Unique())).Id,
                (await UserService.FindOrCreate(StringGenerator.Unique())).Id
            };

            await UserService.UpdateFriends(user.Id, friendIds);
            var result = await UserService.FindFriends(user.Id);
            result.Succeeded.Should().BeTrue();

            var foundFriends = result.Value;
            foundFriends.Should().HaveCount(3);
            foundFriends.Select(e => e.Id).Should().BeEquivalentTo(friendIds);
        }

        [Fact]
        public async Task CanFindUnexistingUserGameState()
        {
            var result = await UserService.FindGameState(Guid.NewGuid());
            result.Succeeded.Should().BeFalse();
            result.ErrorCode.Should().Be(ErrorCode.NotFound);
        }

        [Fact]
        public async Task CanFindExistingUserGameState()
        {
            var user = await UserService.FindOrCreate(StringGenerator.Unique());
            var result = await UserService.FindGameState(user.Id);

            result.Succeeded.Should().BeTrue();

            var state = result.Value;
            state.Should().NotBeNull();
            state.GamesPlayed.Should().Be(0);
            state.Score.Should().Be(0);
        }

        [Fact]
        public async Task CanUpdateGameState()
        {
            var user = await UserService.FindOrCreate(StringGenerator.Unique());
            await UserService.UpdateGameState(user.Id, new State { GamesPlayed = 42, Score = 9000 });
            var result = await UserService.FindGameState(user.Id);

            result.Succeeded.Should().BeTrue();

            var state = result.Value;
            state.Should().NotBeNull();
            state.GamesPlayed.Should().Be(42);
            state.Score.Should().Be(9000);
        }
    }
}
