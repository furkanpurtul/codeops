namespace CodeOps.Domain.Abstractions.BuildingBlocks
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot<TId>
        where TId : class, IStronglyTypedId
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        public int Version { get; private set; } = 0;

        protected AggregateRoot(TId id) : base(id)
        {
        }

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {            
            ArgumentNullException.ThrowIfNull(domainEvent);
            _domainEvents.Add(domainEvent);
            IncrementVersion();
        }

        public void ClearDomainEvents() => _domainEvents.Clear();

        public IReadOnlyCollection<IDomainEvent> DequeueDomainEvents()
        {
            if (_domainEvents.Count == 0) return [];
            var snapshot = _domainEvents.ToArray();
            _domainEvents.Clear();
            return snapshot;
        }

        public void IncrementVersion() => Version++;
    }
}
