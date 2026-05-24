namespace CodeOps.Domain.Abstractions.Violations
{
    public interface IRule
    {
        ViolationKind Kind { get; }

        string Describe();

        bool IsViolated();
    }

    public interface IRule<in TContext>
    {
        ViolationKind Kind { get; }

        string Describe(TContext context);

        bool IsViolatedBy(TContext context);
    }
}
