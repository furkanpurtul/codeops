using CodeOps.Domain.Abstractions.Samples.Entities;

namespace CodeOps.Domain.Abstractions.Samples.Events
{
    /// <summary>
    /// Event raised when an order is cancelled.
    /// </summary>
    public sealed record OrderCancelledEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the identifier of the cancelled order.
        /// </summary>
        public required OrderId OrderId { get; init; }

        /// <summary>
        /// Gets the reason for cancellation.
        /// </summary>
        public string? Reason { get; init; }

        /// <summary>
        /// Gets the timestamp when the order was cancelled.
        /// </summary>
        public DateTime CancelledAt { get; init; } = DateTime.UtcNow;
    }
}
