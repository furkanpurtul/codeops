using CodeOps.Domain.Abstractions.Samples.Entities.Rules;
using CodeOps.Domain.Abstractions.Samples.Enum;
using CodeOps.Domain.Abstractions.Samples.Events;

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
            var order = new Order(id, lines);
            order.RaiseDomainEvent(new OrderCreatedEvent { OrderId = id });
            return order;
        }

        public void Cancel(string? reason = null)
        {
            RuleEngine.Validate(this, new OrderCannotBeCancelledRule());

            Status = OrderStatus.Cancelled;
            RaiseDomainEvent(new OrderCancelledEvent { OrderId = Id, Reason = reason });
        }

        public void AddLine(OrderLine line)
        {
            _lines.Add(line);
        }       
    }
}
