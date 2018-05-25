using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SyboChallenge.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SyboChallenge.Server.Gateway.Rest
{
    public class RestError
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public ICollection<RestErrorDetail> Details { get; set; }
    }

    public class RestErrorDetail
    {
        public string ErrorCode { get; set; }
        public string Target { get; set; }
        public string Message { get; set; }
    }

    public abstract class RestController : ControllerBase
    {
        protected IActionResult OperationResultResponse(OperationResult result)
        {
            if (result.Succeeded)
                throw new InvalidOperationException("OperationResult succeeded");

            if (result.ErrorCode == ErrorCode.NotFound)
                return NotFound();
            if (result.ErrorCode == ErrorCode.InternalError)
                return StatusCode(StatusCodes.Status500InternalServerError);
            if (result.ErrorCode == ErrorCode.BadRequest)
            {
                foreach (var message in result.Errors)
                    ModelState.AddModelError("", message);
                return RestBadRequest();
            }

            throw new InvalidOperationException($"Unknown operation result error code: {result.ErrorCode}");
        }

        protected IActionResult RestBadRequest()
        {
            var error = new RestError
            {
                ErrorCode = "InvalidModel",
                Message = "The request is invalid",
                Details = ModelState.SelectMany(state =>
                    state.Value.Errors.Select(modelError =>
                    new RestErrorDetail
                    {
                        ErrorCode = "Invalid",
                        Target = state.Key,
                        Message = string.IsNullOrWhiteSpace(modelError.ErrorMessage)
                            ? modelError.Exception?.Message
                            : modelError.ErrorMessage
                    }
                    )).ToArray()
            };

            return StatusCode(StatusCodes.Status400BadRequest, error);
        }
    }
}
