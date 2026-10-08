namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed record Violation
    (
        ViolationSource Source,
        ViolationKind Kind,
        string MemberName,
        string Message
    );
}