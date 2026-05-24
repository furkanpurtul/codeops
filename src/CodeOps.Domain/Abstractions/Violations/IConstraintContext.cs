namespace CodeOps.Domain.Abstractions.Violations
{
    public interface IConstraintContext<TSource, TValue>
    {
        TValue Value { get; }

        string MemberName { get; }

        void AddViolation(string message);

        EnsureContext<TSource> Done();
    }
}
