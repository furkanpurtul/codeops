namespace CodeOps.Application.Abstractions.Results
{
    public enum ErrorType
    {
        Unauthorized = 1,
        Forbidden = 2,
        NotFound = 3,
        Validation = 4,
        Conflict = 5,
        Unexpected = 6
    }
}
