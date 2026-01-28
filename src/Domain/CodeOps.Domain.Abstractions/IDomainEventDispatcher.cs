namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Defines a dispatcher for domain events with framework-level support.
    /// </summary>
    /// <remarks>
    /// The dispatcher is responsible for routing domain events to their registered handlers.
    /// Implementations should resolve handlers from a dependency injection container
    /// and invoke them in a consistent manner.
    /// </remarks>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Dispatches a single domain event to all registered handlers.
        /// </summary>
        /// <param name="domainEvent">The domain event to dispatch.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous dispatch operation.</returns>
        /// <remarks>
        /// All handlers for the event type will be invoked sequentially.
        /// Each handler will be attempted even if a previous handler throws an exception.
        /// If one or more handlers fail, their exceptions will be collected and wrapped in an AggregateException.
        /// </remarks>
        Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);

        /// <summary>
        /// Dispatches multiple domain events to their registered handlers.
        /// </summary>
        /// <param name="domainEvents">The collection of domain events to dispatch.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous dispatch operation.</returns>
        /// <remarks>
        /// Events are dispatched in the order they appear in the collection.
        /// Each event's handlers are invoked sequentially before moving to the next event.
        /// </remarks>
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);

        /// <summary>
        /// Dispatches domain events from an aggregate root after dequeuing them.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root whose events should be dispatched.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous dispatch operation.</returns>
        /// <remarks>
        /// This method calls <see cref="IAggregateRoot.DequeueDomainEvents"/> to atomically
        /// retrieve and clear the aggregate's events before dispatching them.
        /// Typically called after successfully persisting the aggregate.
        /// </remarks>
        Task DispatchAsync(IAggregateRoot aggregateRoot, CancellationToken cancellationToken = default);
    }
}
