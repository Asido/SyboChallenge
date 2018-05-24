using SyboChallenge.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyboChallenge.Module.User.Abstraction
{
    public interface IUserService
    {
        Task<IEnumerable<User>> Find();
        Task<User> FindOrCreate(string name);

        Task<OperationResult<State>> FindGameState(Guid userKey);
        Task<OperationResult> UpdateGameState(Guid userKey, State state);

        Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userKey);
        Task<OperationResult> UpdateFriends(Guid userKey, IEnumerable<Guid> friendKeys);
    }
}
