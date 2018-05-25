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

        Task<OperationResult<State>> FindGameState(Guid userId);
        Task<OperationResult> UpdateGameState(Guid userId, State state);

        Task<OperationResult<IEnumerable<Friend>>> FindFriends(Guid userId);
        Task<OperationResult> UpdateFriends(Guid userId, IEnumerable<Guid> friendIds);
    }
}
