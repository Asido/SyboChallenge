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

        Task<OperationResult<State>> FindGameState(Guid userId);
        Task<OperationResult> UpdateGameState(Guid userId, State state);

        Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userId);
        Task<OperationResult> UpdateFriends(Guid userId, IEnumerable<Guid> friendIds);
    }
}
