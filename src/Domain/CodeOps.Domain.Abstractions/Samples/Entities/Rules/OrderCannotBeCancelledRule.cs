using CodeOps.Domain.Abstractions.Samples.Enum;

namespace CodeOps.Domain.Abstractions.Samples.Entities.Rules
{
    public sealed class OrderCannotBeCancelledRule : IRule<Order>
    {
        public string Describe(Order order)
        {
            return "Shipped and delivered orders cannot be cancelled.";
        }

        public bool IsViolatedBy(Order order)
        {
            return order.Status == OrderStatus.Shipped || order.Status == OrderStatus.Delivered;
        }
    }
}
