namespace CodeOps.Domain.Abstractions.Violations
{
    public sealed record DomainViolationSource(string Type, string? Method = null)
    {
        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Method)
                ? Type
                : $"{Type}.{Method}";
        }
    }
}