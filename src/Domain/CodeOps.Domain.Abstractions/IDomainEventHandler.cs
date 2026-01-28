namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Defines a handler for a specific domain event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event this handler processes.</typeparam>
    /// <remarks>
    /// Implement this interface to create handlers that respond to domain events.
    /// Handlers are resolved and invoked by the <see cref="IDomainEventDispatcher"/>.
    /// Multiple handlers can be registered for the same event type.
    /// </remarks>
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        /// <summary>
        /// Handles the specified domain event asynchronously.
        /// </summary>
        /// <param name="domainEvent">The domain event to handle.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>
        /// This method should be idempotent when possible, as events may be dispatched multiple times in failure scenarios.
        /// Exceptions thrown from this method will be caught by the dispatcher and wrapped in an AggregateException.
        /// </remarks>
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
