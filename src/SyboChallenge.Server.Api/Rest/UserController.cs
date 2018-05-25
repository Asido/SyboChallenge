using Microsoft.AspNetCore.Mvc;
using SyboChallenge.Module.User.Abstraction;
using System;
using System.Threading.Tasks;

namespace SyboChallenge.Server.Api.Rest
{
    public class PutFriendsRequest
    {
        public Guid[] Friends { get; set; }
    }

    [Route("/user")]
    public class UserController : RestController
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await userService.Find();
            return Ok(new { Users = users });
        }

        [HttpPost("")]
        public async Task<IActionResult> PostUser([FromBody] User user)
        {
            if (user == null)
            {
                ModelState.AddModelError(nameof(user), "User payload is missing.");
                return RestBadRequest();
            }

            var entity = await userService.FindOrCreate(user.Name);
            return Ok(entity);
        }

        [HttpPut("{userId}/state")]
        public async Task<IActionResult> PutState(Guid userId, [FromBody] State state)
        {
            if (state == null)
            {
                ModelState.AddModelError(nameof(state), "State payload is missing.");
                return RestBadRequest();
            }

            var result = await userService.UpdateGameState(userId, state);
            if (result.Succeeded)
                return Ok();

            return OperationResultResponse(result);
        }

        [HttpGet("{userId}/state")]
        public async Task<IActionResult> GetState(Guid userId)
        {
            var result = await userService.FindGameState(userId);

            if (!result.Succeeded)
                return OperationResultResponse(result);

            if (result.Value == null)
                return NotFound();

            return Ok(result.Value);
        }

        [HttpPut("{userId}/friends")]
        public async Task<IActionResult> PutFriends(Guid userId, [FromBody] PutFriendsRequest model)
        {
            if (model == null || model.Friends == null)
            {
                ModelState.AddModelError(nameof(model.Friends), "Friend payload is missing.");
                return RestBadRequest();
            }

            var result = await userService.UpdateFriends(userId, model.Friends);
            if (result.Succeeded)
                return Ok();

            return OperationResultResponse(result);
        }

        [HttpGet("{userId}/friends")]
        public async Task<IActionResult> GetFriends(Guid userId)
        {
            var result = await userService.FindFriends(userId);
            if (result.Succeeded)
                return Ok(new { Friends = result.Value });

            return OperationResultResponse(result);
        }
    }
}
