using SyboChallenge.Core;
using SyboChallenge.Module.User.Abstraction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyboChallenge.Module.User.Service
{
    public class UserService : IUserService
    {
        private readonly IUserStore store;

        public UserService(IUserStore store)
        {
            this.store = store;
        }

        public Task<IEnumerable<Abstraction.User>> Find() => store.Find();

        public async Task<Abstraction.User> FindOrCreate(string name)
        {
            var user = await store.Find(name);

            if (user == null)
            {
                user = new Abstraction.User
                {
                    Id = Guid.NewGuid(),
                    Name = name
                };
                await store.Insert(user);
            }

            return user;
        }

        public Task<OperationResult<State>> FindGameState(Guid userId) => store.FindGameState(userId);
        public Task<OperationResult> UpdateGameState(Guid userId, State state) => store.UpdateGameState(userId, state);

        public Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userId) => store.FindFriends(userId);
        public Task<OperationResult> UpdateFriends(Guid userId, IEnumerable<Guid> friendIds) => store.UpdateFriends(userId, friendIds);
    }
}
