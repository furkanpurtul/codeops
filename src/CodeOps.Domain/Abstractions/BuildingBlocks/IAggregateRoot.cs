namespace CodeOps.Domain.Abstractions.BuildingBlocks
{
    public interface IAggregateRoot : IEntity
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        void ClearDomainEvents();

        IReadOnlyCollection<IDomainEvent> DequeueDomainEvents();
    }

    public interface IAggregateRoot<out TId> : IAggregateRoot, IEntity<TId>
        where TId : IStronglyTypedId
    {
    }
}
