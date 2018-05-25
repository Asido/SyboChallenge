using Microsoft.WindowsAzure.Storage.Table;
using SyboChallenge.Core;
using SyboChallenge.Core.AzureTableStorage;
using SyboChallenge.Module.User.Abstraction;
using SyboChallenge.Module.User.Store.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyboChallenge.Module.User.Store
{
    public class UserStore : IUserStore
    {
        private readonly CloudTable userTable;
        private readonly CloudTable userNameTable;

        public UserStore(AzureTableProvider tables)
        {
            userTable = tables.UserTable;
            userNameTable = tables.UserNameTable;
        }

        public async Task<IEnumerable<Abstraction.User>> Find()
        {
            var entities = await userNameTable.All<UserNameEntity>();
            var users = entities.Select(e => new Abstraction.User { Key = e.Key, Name = e.Name });
            return users;
        }

        public async Task<Abstraction.User> Find(string name)
        {
            var entity = await userNameTable.Find<UserNameEntity>(UserNameEntity.FormatPartitionKey(name), UserNameEntity.FormatRowKey());

            if (entity == null)
                return null;

            return new Abstraction.User { Key = entity.Key, Name = entity.Name };
        }

        public async Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userKey)
        {
            var entity = await userTable.Find<UserEntity>(
                UserEntity.FormatPartitionKey(userKey),
                UserEntity.FormatRowKey(),
                new List<string> { nameof(UserEntity.FriendsJson) });
            if (entity == null)
                return OperationResult<IEnumerable<Friend>>.Failed(ErrorCode.NotFound, "User not found");

            var tasks = entity.Friends.Select(friendKey =>
            {
                return userTable.Find<UserEntity>(
                    UserEntity.FormatPartitionKey(friendKey),
                    UserEntity.FormatRowKey(),
                    new List<string> { nameof(UserEntity.Name), nameof(UserEntity.Highscore) });
            });

            await Task.WhenAll(tasks);

            var friends = tasks
                .Select(task => task.Result)
                .Where(e => e != null)
                .Select(e => new Friend { Key = e.Key, Name = e.Name, Highscore = e.Highscore });
            return OperationResult<IEnumerable<Friend>>.Success(friends);
        }

        public async Task<OperationResult<State>> FindGameState(Guid userKey)
        {
            var entity = await userTable.Find<UserEntity>(
                UserEntity.FormatPartitionKey(userKey),
                UserEntity.FormatRowKey(),
                new List<string> { nameof(UserEntity.GamesPlayed), nameof(UserEntity.Highscore) });

            if (entity == null)
                return OperationResult<State>.Failed(ErrorCode.NotFound, "User not found");

            return OperationResult<State>.Success(new State { GamesPlayed = entity.GamesPlayed, Score = entity.Highscore });
        }

        public async Task Insert(Abstraction.User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            await Task.WhenAll(
                userTable.ExecuteAsync(TableOperation.Insert(new UserEntity(user.Key, user.Name, 0, 0))),
                userNameTable.ExecuteAsync(TableOperation.Insert(new UserNameEntity(user.Name, user.Key))));
        }

        public async Task<OperationResult> UpdateFriends(Guid userKey, IEnumerable<Guid> friendKeys)
        {
            var entity = new DynamicTableEntity(UserEntity.FormatPartitionKey(userKey), UserEntity.FormatRowKey()) { ETag = "*" };
            entity.Properties.Add(nameof(UserEntity.FriendsJson), new EntityProperty(UserEntity.FormatFriendsJson(friendKeys)));
            await userTable.ExecuteAsync(TableOperation.Merge(entity));

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateGameState(Guid userKey, State state)
        {
            var entity = new DynamicTableEntity(UserEntity.FormatPartitionKey(userKey), UserEntity.FormatRowKey()) { ETag = "*" };
            entity.Properties.Add(nameof(UserEntity.GamesPlayed), new EntityProperty(state.GamesPlayed));
            entity.Properties.Add(nameof(UserEntity.Highscore), new EntityProperty(state.Score));
            await userTable.ExecuteAsync(TableOperation.Merge(entity));

            return OperationResult.Success;
        }
    }
}
