using SyboChallenge.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyboChallenge.Module.User.Abstraction
{
    public interface IUserStore
    {
        Task<IEnumerable<User>> Find();
        Task<User> Find(string name);
        Task Insert(User user);

        Task<OperationResult<State>> FindGameState(Guid userKey);
        Task<OperationResult> UpdateGameState(Guid userKey, State state);

        Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userKey);
        Task<OperationResult> UpdateFriends(Guid userKey, IEnumerable<Guid> friendKeys);
    }
}
