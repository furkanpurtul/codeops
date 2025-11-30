namespace CodeOps.Domain.Abstractions.Samples.ValueObjects.Rules
{

    public sealed class CurrencyIso4217Rule : IRule<Money>
    {
        private static readonly HashSet<string> Iso4217Currencies = new(StringComparer.Ordinal)
        {

            "USD","EUR","GBP","TRY","JPY","CHF","CAD","AUD","NZD","SEK","NOK","DKK","CNY","HKD","SGD","BRL",
            "RUB","PLN","CZK","HUF","RON","UAH","MXN","CLP","ARS","ZAR","INR","AED","SAR","QAR","KWD","BHD",
            "KRW","THB","IDR","MYR","PHP","ILS","NGN","COP","PEN","VND"
        };

        public string Describe(Money context) => "Currency must be a valid ISO 4217 code";
        public bool IsViolatedBy(Money m)
            => string.IsNullOrWhiteSpace(m.Currency)
               || m.Currency.Length != 3
               || !Iso4217Currencies.Contains(m.Currency);
    }
}
