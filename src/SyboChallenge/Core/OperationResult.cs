using System.Collections.Generic;
using System.Linq;

namespace SyboChallenge.Core
{
    public enum ErrorCode
    {
        BadRequest = 400,
        NotFound = 404,
        InternalError = 500
    }

    public class OperationResult
    {
        public bool Succeeded => !Errors.Any();

        public ErrorCode ErrorCode { get; protected set; }
        public IEnumerable<string> Errors { get; protected set; }

        protected OperationResult() { }

        private static readonly OperationResult successResult = new OperationResult { Errors = new string[0] };

        public static OperationResult Success => successResult;

        public static OperationResult Failed(ErrorCode code, string error) =>
            new OperationResult { ErrorCode = code, Errors = new[] { error } };

        public static OperationResult Failed(ErrorCode code, IEnumerable<string> errors) =>
            new OperationResult { ErrorCode = code, Errors = errors };
    }

    public class OperationResult<TResult> : OperationResult
    {
        public readonly TResult Value;

        private OperationResult() { }

        public OperationResult(TResult value)
        {
            Value = value;
            Errors = new string[0];
        }

        public new static OperationResult<TResult> Success(TResult result) => new OperationResult<TResult>(result);

        public new static OperationResult<TResult> Failed(ErrorCode code, string error) =>
            new OperationResult<TResult> { ErrorCode = code, Errors = new[] { error } };

        public new static OperationResult<TResult> Failed(ErrorCode code, IEnumerable<string> errors) =>
            new OperationResult<TResult> { ErrorCode = code, Errors = errors };
    }
}
