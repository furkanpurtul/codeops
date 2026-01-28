using CodeOps.Domain.Abstractions.Samples.Entities;

namespace CodeOps.Domain.Abstractions.Samples.Events
{
    /// <summary>
    /// Event raised when an order is created.
    /// </summary>
    public sealed record OrderCreatedEvent : IDomainEvent
    {
        /// <summary>
        /// Gets the identifier of the created order.
        /// </summary>
        public required OrderId OrderId { get; init; }

        /// <summary>
        /// Gets the timestamp when the order was created.
        /// </summary>
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    }
}
