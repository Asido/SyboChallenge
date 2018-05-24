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

        public UserStore(AzureTableProvider tables)
        {
            userTable = tables.UserTable;
        }

        public async Task<IEnumerable<Abstraction.User>> Find()
        {
            var columns = new[] { nameof(UserEntity.Name) };
            var entities = await userTable.All<UserEntity>(columns);
            var users = entities.Select(e => new Abstraction.User { Key = e.Key, Name = e.Name });
            return users;
        }

        public async Task<Abstraction.User> Find(string name)
        {
            var rowKey = UserEntity.FormatRowKey(name);
            var columns = new[] { nameof(UserEntity.Name) };
            var entity = await userTable.QueryByRowKeySingleOrDefault<UserEntity>(rowKey, columns);

            if (entity == null)
                return null;

            return new Abstraction.User { Key = entity.Key, Name = entity.Name };
        }

        public async Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userKey)
        {
            var entity = await userTable.QueryByPartitionKeySingleOrDefault<UserEntity>(
                UserEntity.FormatPartitionKey(userKey), new[] { nameof(UserEntity.FriendsJson) });
            if (entity == null)
                return OperationResult<IEnumerable<Friend>>.Failed(ErrorCode.NotFound, "User not found");

            var tasks = entity.Friends.Select(friendKey =>
            {
                var partitionKey = UserEntity.FormatPartitionKey(friendKey);
                var columns = new string[] { nameof(UserEntity.Name), nameof(UserEntity.Highscore) };
                return userTable.QueryByPartitionKeySingleOrDefault<UserEntity>(partitionKey, columns);
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
            var partitionKey = UserEntity.FormatPartitionKey(userKey);
            var columns = new[] { nameof(UserEntity.GamesPlayed), nameof(UserEntity.Highscore) };
            var entity = await userTable.QueryByPartitionKeySingleOrDefault<UserEntity>(partitionKey, columns);

            if (entity == null)
                return OperationResult<State>.Failed(ErrorCode.NotFound, "User not found");

            return OperationResult<State>.Success(new State { GamesPlayed = entity.GamesPlayed, Score = entity.Highscore });
        }

        public async Task Insert(Abstraction.User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var entity = new UserEntity(user.Key, user.Name, 0, 0);
            await userTable.ExecuteAsync(TableOperation.Insert(entity));
        }

        public async Task<OperationResult> UpdateFriends(Guid userKey, IEnumerable<Guid> friendKeys)
        {
            var user = await userTable.QueryByPartitionKeySingleOrDefault<UserEntity>(UserEntity.FormatPartitionKey(userKey), new[] { nameof(UserEntity.Name) });
            if (user == null)
                return OperationResult.Failed(ErrorCode.NotFound, "User not found");

            user.Friends = friendKeys.ToList();

            var entity = new DynamicTableEntity(user.PartitionKey, user.RowKey) { ETag = "*" };
            entity.Properties.Add(nameof(UserEntity.FriendsJson), new EntityProperty(user.FriendsJson));
            await userTable.ExecuteAsync(TableOperation.Merge(entity));

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateGameState(Guid userKey, State state)
        {
            var user = await userTable.QueryByPartitionKeySingleOrDefault<UserEntity>(UserEntity.FormatPartitionKey(userKey), new[] { nameof(UserEntity.Name) });
            if (user == null)
                return OperationResult.Failed(ErrorCode.NotFound, "User not found");

            var entity = new DynamicTableEntity(user.PartitionKey, user.RowKey) { ETag = "*" };
            entity.Properties.Add(nameof(UserEntity.GamesPlayed), new EntityProperty(state.GamesPlayed));
            entity.Properties.Add(nameof(UserEntity.Highscore), new EntityProperty(state.Score));
            await userTable.ExecuteAsync(TableOperation.Merge(entity));

            return OperationResult.Success;
        }
    }
}
