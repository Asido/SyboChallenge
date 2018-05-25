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
        private readonly AzureTableProvider db;

        public UserStore(AzureTableProvider db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<Abstraction.User>> Find()
        {
            var entities = await db.UserNameTable.All<UserNameEntity>();
            var users = entities.Select(e => new Abstraction.User { Id = e.Id, Name = e.Name });
            return users;
        }

        public async Task<Abstraction.User> Find(string name)
        {
            var entity = await db.UserNameTable.Find<UserNameEntity>(UserNameEntity.FormatPartitionKey(name), UserNameEntity.FormatRowKey());

            if (entity == null)
                return null;

            return new Abstraction.User { Id = entity.Id, Name = entity.Name };
        }

        public async Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userId)
        {
            var entity = await db.UserTable.Find<UserEntity>(
                UserEntity.FormatPartitionKey(userId),
                UserEntity.FormatRowKey(),
                new List<string> { nameof(UserEntity.FriendsJson) });
            if (entity == null)
                return OperationResult<IEnumerable<Friend>>.Failed(ErrorCode.NotFound, "User not found");

            var tasks = entity.Friends.Select(friendId =>
            {
                return db.UserTable.Find<UserEntity>(
                    UserEntity.FormatPartitionKey(friendId),
                    UserEntity.FormatRowKey(),
                    new List<string> { nameof(UserEntity.Name), nameof(UserEntity.Highscore) });
            });

            await Task.WhenAll(tasks);

            var friends = tasks
                .Select(task => task.Result)
                .Where(e => e != null)
                .Select(e => new Friend { Id = e.Id, Name = e.Name, Highscore = e.Highscore });
            return OperationResult<IEnumerable<Friend>>.Success(friends);
        }

        public async Task<OperationResult<State>> FindGameState(Guid userId)
        {
            var entity = await db.UserTable.Find<UserEntity>(
                UserEntity.FormatPartitionKey(userId),
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
                db.UserTable.ExecuteAsync(TableOperation.Insert(new UserEntity(user.Id, user.Name, 0, 0))),
                db.UserNameTable.ExecuteAsync(TableOperation.Insert(new UserNameEntity(user.Name, user.Id))));
        }

        public async Task<OperationResult> UpdateFriends(Guid userId, IEnumerable<Guid> friendIds)
        {
            var entity = new DynamicTableEntity(UserEntity.FormatPartitionKey(userId), UserEntity.FormatRowKey()) { ETag = "*" };
            entity.Properties.Add(nameof(UserEntity.FriendsJson), new EntityProperty(UserEntity.FormatFriendsJson(friendIds)));
            await db.UserTable.ExecuteAsync(TableOperation.Merge(entity));

            return OperationResult.Success;
        }

        public async Task<OperationResult> UpdateGameState(Guid userId, State state)
        {
            var entity = new DynamicTableEntity(UserEntity.FormatPartitionKey(userId), UserEntity.FormatRowKey()) { ETag = "*" };
            entity.Properties.Add(nameof(UserEntity.GamesPlayed), new EntityProperty(state.GamesPlayed));
            entity.Properties.Add(nameof(UserEntity.Highscore), new EntityProperty(state.Score));
            await db.UserTable.ExecuteAsync(TableOperation.Merge(entity));

            return OperationResult.Success;
        }
    }
}
