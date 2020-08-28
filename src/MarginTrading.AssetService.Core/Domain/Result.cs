using System;

namespace MarginTrading.AssetService.Core.Domain
{
    public class Result<TValue, TError>
        where TValue : class
        where TError : struct, Enum 
    {
        public TValue Value { get; }
        public TError? Error { get; }

        public bool IsFailed => Error.HasValue;
        
        public bool IsSuccess => !IsFailed;

        public Result(TValue value)
        {
            Value = value ?? throw new ArgumentException($"{nameof(value)} cannot be null");
        }

        public Result(TError error)
        {
            Error = error;
        }

        public Result<TError> ToResultWithoutValue()
        {
            return new Result<TError>(Error);
        }
    }

    public class Result<TError> where TError : struct, Enum
    {
        public TError? Error { get; }
        public bool IsFailed => Error.HasValue;
        
        public bool IsSuccess => !IsFailed;

        public Result(TError? error = null)
        {
            Error = error;
        }
    }
}