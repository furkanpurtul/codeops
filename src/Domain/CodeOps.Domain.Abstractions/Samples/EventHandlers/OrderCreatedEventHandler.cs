using CodeOps.Domain.Abstractions.Samples.Events;

namespace CodeOps.Domain.Abstractions.Samples.EventHandlers
{
    /// <summary>
    /// Sample handler for OrderCreatedEvent that logs the event.
    /// </summary>
    /// <remarks>
    /// This is a sample implementation showing how to create domain event handlers.
    /// In a real application, this might send notifications, update read models, or trigger other processes.
    /// </remarks>
    public sealed class OrderCreatedEventHandler : IDomainEventHandler<OrderCreatedEvent>
    {
        /// <inheritdoc/>
        public Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // In a real application, you might:
            // - Send an email notification
            // - Update a read model/projection
            // - Publish to a message bus
            // - Log to an audit trail
            
            Console.WriteLine($"[OrderCreatedEventHandler] Order {domainEvent.OrderId.Value} was created at {domainEvent.CreatedAt:O}");
            
            return Task.CompletedTask;
        }
    }
}
