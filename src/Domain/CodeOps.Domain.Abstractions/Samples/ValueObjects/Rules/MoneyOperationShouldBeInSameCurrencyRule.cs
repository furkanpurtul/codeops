namespace CodeOps.Domain.Abstractions.Samples.ValueObjects.Rules
{
    public sealed class MoneyOperationShouldBeInSameCurrencyRule : IRule<(Money a, Money b)>
    {
        public string Describe((Money a, Money b) context)
        {
            return $"Currency mismatch: {context.a.Currency} vs {context.b.Currency}";
        }

        public bool IsViolatedBy((Money a, Money b) context)
        {
            return !string.Equals(context.a.Currency, context.b.Currency, StringComparison.Ordinal);
        }
    }
}
