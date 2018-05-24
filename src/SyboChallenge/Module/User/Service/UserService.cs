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
                    Key = Guid.NewGuid(),
                    Name = name
                };
                await store.Insert(user);
            }

            return user;
        }

        public Task<OperationResult<State>> FindGameState(Guid userKey) => store.FindGameState(userKey);
        public Task<OperationResult> UpdateGameState(Guid userKey, State state) => store.UpdateGameState(userKey, state);

        public Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userKey) => store.FindFriends(userKey);
        public Task<OperationResult> UpdateFriends(Guid userKey, IEnumerable<Guid> friendKeys) => store.UpdateFriends(userKey, friendKeys);
    }
}
