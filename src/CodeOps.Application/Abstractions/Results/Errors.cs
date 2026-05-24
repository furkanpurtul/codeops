namespace CodeOps.Application.Abstractions.Results
{
    public static class Errors
    {
        public static Error Unauthorized(string code, string message, IReadOnlyDictionary<string, object?>? metadata = null)
            => new(code, message, ErrorType.Unauthorized, metadata);

        public static Error Forbidden(string code, string message, IReadOnlyDictionary<string, object?>? metadata = null)
            => new(code, message, ErrorType.Forbidden, metadata);

        public static Error NotFound(string code, string message, IReadOnlyDictionary<string, object?>? metadata = null)
            => new(code, message, ErrorType.NotFound, metadata);

        public static Error Validation(string code, string message, IReadOnlyDictionary<string, object?>? metadata = null)
            => new(code, message, ErrorType.Validation, metadata);

        public static Error Conflict(string code, string message, IReadOnlyDictionary<string, object?>? metadata = null)
            => new(code, message, ErrorType.Conflict, metadata);

        public static Error Unexpected(string code, string message, IReadOnlyDictionary<string, object?>? metadata = null)
            => new(code, message, ErrorType.Unexpected, metadata);
    }
}
