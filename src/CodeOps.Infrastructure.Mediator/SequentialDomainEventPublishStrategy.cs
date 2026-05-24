using CodeOps.Application.Abstractions.Messaging;
using CodeOps.Domain.Abstractions.BuildingBlocks;

namespace CodeOps.Infrastructure.Mediator
{
    internal sealed class SequentialDomainEventPublishStrategy<TEvent> : IDomainEventPublishStrategy<TEvent>
        where TEvent : class, IDomainEvent
    {
        public async Task PublishAsync(IReadOnlyList<IDomainEventHandler<TEvent>> handlers, TEvent domainEvent, CancellationToken cancellationToken)
        {
            for (var index = 0; index < handlers.Count; index++)
            {
                await handlers[index].HandleAsync(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
