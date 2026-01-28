using CodeOps.Domain.Abstractions.Samples.Events;

namespace CodeOps.Domain.Abstractions.Samples.EventHandlers
{
    /// <summary>
    /// Sample handler for OrderCancelledEvent that logs the event.
    /// </summary>
    /// <remarks>
    /// This is a sample implementation showing how to create domain event handlers.
    /// In a real application, this might refund payments, notify customers, or update inventory.
    /// </remarks>
    public sealed class OrderCancelledEventHandler : IDomainEventHandler<OrderCancelledEvent>
    {
        /// <inheritdoc/>
        public Task HandleAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // In a real application, you might:
            // - Process refunds
            // - Restore inventory
            // - Send cancellation notification
            // - Update analytics
            
            var reason = string.IsNullOrEmpty(domainEvent.Reason) 
                ? "No reason provided" 
                : domainEvent.Reason;
                
            Console.WriteLine($"[OrderCancelledEventHandler] Order {domainEvent.OrderId.Value} was cancelled at {domainEvent.CancelledAt:O}. Reason: {reason}");
            
            return Task.CompletedTask;
        }
    }
}
