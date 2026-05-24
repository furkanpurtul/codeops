namespace CodeOps.Application.Abstractions.Results
{
    public class Result
    {
        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && !error.IsNone)
                throw new InvalidOperationException("A successful result cannot contain an error.");

            if (!isSuccess && error.IsNone)
                throw new InvalidOperationException("A failed result must contain an error.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new(false, error);
        }

        public static Result<T> Success<T>(T value) => Result<T>.Success(value);

        public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);

        public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure)
        {
            ArgumentNullException.ThrowIfNull(onSuccess);
            ArgumentNullException.ThrowIfNull(onFailure);

            return IsSuccess
                ? onSuccess()
                : onFailure(Error);
        }

        public static implicit operator Result(Error error) => Failure(error);
    }

    public sealed class Result<T> : Result
    {
        private readonly T? _value;

        private Result(T value) : base(true, Error.None)
        {
            _value = value;
        }

        private Result(Error error) : base(false, error)
        {
            _value = default;
        }

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("Cannot access the value of a failed result.");

        public T? ValueOrDefault => IsSuccess ? _value : default;

        public static Result<T> Success(T value) => new(value);

        public static new Result<T> Failure(Error error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new(error);
        }

        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
        {
            ArgumentNullException.ThrowIfNull(onSuccess);
            ArgumentNullException.ThrowIfNull(onFailure);

            return IsSuccess
                ? onSuccess(Value)
                : onFailure(Error);
        }

        public static implicit operator Result<T>(T value) => Success(value);

        public static implicit operator Result<T>(Error error) => Failure(error);
    }
}
