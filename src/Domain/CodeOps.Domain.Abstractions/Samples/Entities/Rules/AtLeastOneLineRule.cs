namespace CodeOps.Domain.Abstractions.Samples.Entities.Rules
{
    public sealed class AtLeastOneLineRule : IRule<Order>
    {
        public string Describe(Order order) => "Order must have at least one line.";

        public bool IsViolatedBy(Order order) => order.Lines.Count == 0;
    }
}
