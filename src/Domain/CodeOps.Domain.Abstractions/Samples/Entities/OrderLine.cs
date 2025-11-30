using CodeOps.Domain.Abstractions.Samples.ValueObjects;

namespace CodeOps.Domain.Abstractions.Samples.Entities
{
    public sealed class OrderLine : Entity<OrderLine, OrderLineId>
    {
        public string Sku { get; }
        public int Quantity { get; }
        public Money UnitPrice { get; }

        private OrderLine(OrderLineId id, string sku, int quantity, Money unitPrice)
            : base(id, [new SkuNotEmptyRule(), new QuantityPositiveRule(), new UnitPriceNotNullRule()])
        {
            Sku = sku;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public static OrderLine Create(OrderLineId id, string sku, int quantity, Money unitPrice)
        {
            return new OrderLine(id, sku, quantity, unitPrice);
        }

        private sealed class SkuNotEmptyRule : IRule<OrderLine>
        {
            public string Describe(OrderLine context)
            {
                return "SKU is required.";
            }

            public bool IsViolatedBy(OrderLine l) => string.IsNullOrWhiteSpace(l.Sku);
        }

        private sealed class QuantityPositiveRule : IRule<OrderLine>
        {
            public string Describe(OrderLine context)
            {
                return "Quantity must be > 0";
            }

            public bool IsViolatedBy(OrderLine l) => l.Quantity <= 0;
        }

        private sealed class UnitPriceNotNullRule : IRule<OrderLine>
        {
            public string Describe(OrderLine context)
            {
                return "Unit price is required.";
            }

            public bool IsViolatedBy(OrderLine l) => l.UnitPrice is null;
        }
    }
}
