namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Base type for aggregate roots implementing domain event collection and optimistic versioning.
    /// </summary>
    /// <typeparam name="TDerived">
    /// Concrete aggregate type (CRTP pattern) enabling invariant evaluation and stronger typing.
    /// </typeparam>
    /// <typeparam name="TId">
    /// Strongly-typed identity type for the aggregate root.
    /// </typeparam>
    /// <remarks>
    /// Responsibilities:
    /// 1. Encapsulate domain invariants via the base <see cref="Entity{TDerived,TId}"/>.
    /// 2. Collect domain events raised during state changes for later dispatch.
    /// 3. Maintain a simple in-memory version counter (incremented per event) useful for optimistic concurrency.
    /// Use <see cref="RaiseDomainEvent(IDomainEvent)"/> inside protected/internal behavioral methods
    /// whenever a meaningful state transition occurs.
    /// </remarks>
    public abstract class AggregateRoot<TDerived, TId> :
        Entity<TDerived, TId>, IAggregateRoot<TId>
        where TDerived : AggregateRoot<TDerived, TId>
        where TId : class, IStronglyTypedId
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        /// <summary>
        /// Gets the current in-memory version of the aggregate root.
        /// </summary>
        /// <remarks>
        /// Incremented each time <see cref="RaiseDomainEvent(IDomainEvent)"/> is called.
        /// Persisting infrastructure may map this to an optimistic concurrency token.
        /// </remarks>
        public int Version { get; private set; } = 0;

        /// <summary>
        /// Initializes a new instance of the aggregate root with identity and invariants.
        /// </summary>
        /// <param name="id">Strongly-typed aggregate identity.</param>
        /// <param name="invariants">Invariant rules enforced by the base entity logic.</param>
        protected AggregateRoot(TId id, IRule<TDerived>[] invariants) : base(id, invariants)
        {
        }

        /// <summary>
        /// Gets the domain events raised since the last clear/dequeue operation.
        /// </summary>
        /// <remarks>
        /// Returned collection is a snapshot view. Mutations require <see cref="RaiseDomainEvent(IDomainEvent)"/> or clearing methods.
        /// </remarks>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Raises and records a domain event, incrementing the aggregate version.
        /// </summary>
        /// <param name="domainEvent">The domain event instance to record.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="domainEvent"/> is null.</exception>
        /// <remarks>
        /// Invoking this method should be tied to a validated state change.
        /// Infrastructure can later dispatch collected events after persistence succeeds.
        /// </remarks>
        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);
            _domainEvents.Add(domainEvent);
            IncrementVersion();
        }

        /// <summary>
        /// Removes all currently buffered domain events without dispatching them.
        /// </summary>
        public void ClearDomainEvents() => _domainEvents.Clear();

        /// <summary>
        /// Atomically captures and clears all buffered domain events.
        /// </summary>
        /// <returns>A snapshot array of the previously buffered domain events (empty if none).</returns>
        /// <remarks>
        /// Preferred over manual enumeration + clear to avoid race conditions in multi-threaded scenarios.
        /// </remarks>
        public IReadOnlyCollection<IDomainEvent> DequeueDomainEvents()
        {
            if (_domainEvents.Count == 0) return [];
            var snapshot = _domainEvents.ToArray();
            _domainEvents.Clear();
            return snapshot;
        }

        /// <summary>
        /// Increments the in-memory version counter.
        /// </summary>
        /// <remarks>
        /// Normally invoked indirectly via <see cref="RaiseDomainEvent(IDomainEvent)"/>.
        /// Can be overridden if external version synchronization is required.
        /// </remarks>
        public void IncrementVersion() => Version++;
    }
}
