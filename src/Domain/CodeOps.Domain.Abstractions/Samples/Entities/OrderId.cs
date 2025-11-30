namespace CodeOps.Domain.Abstractions.Samples.Entities
{
    public sealed record OrderId : StronglyTypedId<Guid>
    {
        public OrderId(Guid value) : base(value) { }
        public static OrderId New() => new(Guid.NewGuid());
    }
}
