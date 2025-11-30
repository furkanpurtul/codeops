using CodeOps.Domain.Abstractions.Samples.ValueObjects.Rules;
using System.Globalization;

namespace CodeOps.Domain.Abstractions.Samples.ValueObjects
{
    public sealed class Money : ValueObject<Money>
    {
        public decimal Amount { get; }

        public string Currency { get; } 

        private Money(decimal amount, string currency) : 
            base([new AmountNonNegativeRule(), new CurrencyIso4217Rule()])
        {
            Amount = amount;
            Currency = currency; 
        }

        public static Money Of(decimal amount, string currency)
        {
            var normalized = NormalizeCurrency(currency);
            return new Money(amount, normalized);
        }

        public static Money Zero(string currency) => Of(0m, currency);

        public static Money operator +(Money a, Money b)
        {
            EnsureSameCurrency(a, b);
            return Of(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            EnsureSameCurrency(a, b);
            return Of(a.Amount - b.Amount, a.Currency);
        }

        public static Money operator *(Money a, decimal factor)
        {
            if (factor < 0m)
                throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be >= 0 (negative would produce invalid Money).");

            return Of(a.Amount * factor, a.Currency);
        }

        public static Money operator *(decimal factor, Money a) => a * factor;

        private static void EnsureSameCurrency(Money a, Money b) =>
            RuleEngine.Validate((a, b), new MoneyOperationShouldBeInSameCurrencyRule());

        private static string NormalizeCurrency(string currency)
        {
            var trimmed = currency.Trim().ToUpperInvariant();
            return trimmed;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency; // already normalized
        }

        public override string ToString() => $"{Amount.ToString(CultureInfo.InvariantCulture)} {Currency}";
    }
}