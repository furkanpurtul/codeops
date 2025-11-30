namespace CodeOps.Domain.Abstractions.Samples.Entities
{
    public sealed record OrderLineId : StronglyTypedId<Guid>
    {
        public OrderLineId(Guid value) : base(value) { }

        public static OrderLineId New() => new(Guid.NewGuid());
    }
}
