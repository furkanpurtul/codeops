using CodeOps.Domain.Abstractions.Samples.Entities.Rules;
using CodeOps.Domain.Abstractions.Samples.Enum;

namespace CodeOps.Domain.Abstractions.Samples.Entities
{
    public sealed class Order : AggregateRoot<Order, OrderId>
    {
        private readonly List<OrderLine> _lines = [];
        public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();

        public OrderStatus Status { get; private set; }

        private Order(OrderId id, IEnumerable<OrderLine> lines) : base(id, [new AtLeastOneLineRule(), new AllLinesSameCurrencyRule()])
        {
            Status = OrderStatus.Pending;
            if (lines != null)
            {
                _lines.AddRange(lines);
            }
        }

        public static Order Create(OrderId id, IEnumerable<OrderLine> lines)
        {
            return new Order(id, lines);
        }

        public void Cancel()
        {
            RuleEngine.Validate(this, new OrderCannotBeCancelledRule());

            Status = OrderStatus.Cancelled;
        }

        public void AddLine(OrderLine line)
        {
            _lines.Add(line);
        }       
    }
}
