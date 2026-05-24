namespace CodeOps.Application.Abstractions.Results
{
    public sealed record Error(string Code, string Message, ErrorType Type, IReadOnlyDictionary<string, object?>? Metadata = null)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Unexpected);

        public bool IsNone => this == None;
    }
}
