namespace CodeOps.Domain.Abstractions.Samples.ValueObjects.Rules
{
    public sealed class AmountNonNegativeRule : IRule<Money>
    {
        public string Describe(Money context)
        {
            return $"The amount '{context.Amount}' is negative. Must be >= 0";
        }

        public bool IsViolatedBy(Money m) => m.Amount < 0m;
    }
}
