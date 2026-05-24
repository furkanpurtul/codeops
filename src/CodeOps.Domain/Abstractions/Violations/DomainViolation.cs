namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed record DomainViolation
    (
        DomainViolationSource Source,
        ViolationKind Kind,
        string MemberName,
        string Message
    );
}