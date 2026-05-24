using CodeOps.Domain.Abstractions.BuildingBlocks;

namespace CodeOps.Application.Abstractions.Messaging
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
    }
}
