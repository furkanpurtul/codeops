using CodeOps.Application.Abstractions.Messaging;
using CodeOps.Domain.Abstractions.BuildingBlocks;

namespace CodeOps.Infrastructure.Mediator
{
    internal interface IDomainEventPublishStrategy<TEvent> where TEvent : class, IDomainEvent
    {
        Task PublishAsync(IReadOnlyList<IDomainEventHandler<TEvent>> handlers, TEvent domainEvent, CancellationToken cancellationToken);
    }
}
