namespace CodeOps.Domain.Abstractions.Samples.Entities.Rules
{
    public sealed class AllLinesSameCurrencyRule : IRule<Order>
    {
        public string Describe(Order order) => "All order lines must share the same currency.";

        public bool IsViolatedBy(Order order)
        {
            if (order.Lines.Count <= 1)
            {
                return false;
            }

            var firstCurrency = order.Lines.First().UnitPrice.Currency.ToUpperInvariant();
            foreach (var line in order.Lines.Skip(1))
            {
                if (!string.Equals(firstCurrency, line.UnitPrice.Currency.ToUpperInvariant(), StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
