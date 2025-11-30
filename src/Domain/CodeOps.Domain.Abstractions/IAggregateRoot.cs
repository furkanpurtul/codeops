namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Contract for aggregate roots exposing domain event lifecycle without a concrete id type.
    /// </summary>
    /// <remarks>
    /// Allows infrastructure to collect and dispatch domain events raised during state changes.
    /// Use the generic <see cref="IAggregateRoot{TId}"/> when the concrete identity type is required.
    /// </remarks>
    public interface IAggregateRoot : IEntity
    {
        /// <summary>
        /// Gets the domain events currently recorded and not yet dispatched.
        /// </summary>
        /// <remarks>
        /// The returned collection is a snapshot and should be treated as read-only.
        /// </remarks>
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

        /// <summary>
        /// Clears all recorded domain events without returning them.
        /// </summary>
        /// <remarks>
        /// Use when events should be discarded (e.g., rollback scenarios).
        /// </remarks>
        void ClearDomainEvents();

        /// <summary>
        /// Returns the currently recorded domain events and clears the internal buffer (pull pattern).
        /// </summary>
        /// <returns>A snapshot of the buffered domain events; empty if none.</returns>
        /// <remarks>
        /// Preferred for atomic capture and clear before dispatching through the outbox or mediator.
        /// </remarks>
        IReadOnlyCollection<IDomainEvent> DequeueDomainEvents();
    }

    /// <summary>
    /// Strongly-typed aggregate root contract exposing the concrete id type.
    /// </summary>
    /// <typeparam name="TId">The strongly-typed identity type implementing <see cref="IStronglyTypedId"/>.</typeparam>
    public interface IAggregateRoot<out TId> : IAggregateRoot, IEntity<TId>
        where TId : IStronglyTypedId
    {
    }
}


