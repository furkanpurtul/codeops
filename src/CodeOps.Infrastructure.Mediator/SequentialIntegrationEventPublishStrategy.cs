using CodeOps.Application.Abstractions.Messaging;

namespace CodeOps.Infrastructure.Mediator
{
    internal sealed class SequentialIntegrationEventPublishStrategy<TEvent> : IIntegrationEventPublishStrategy<TEvent>
        where TEvent : class, IIntegrationEvent
    {
        public async Task PublishAsync(IReadOnlyList<IIntegrationEventHandler<TEvent>> handlers, TEvent integrationEvent, CancellationToken cancellationToken)
        {
            for (var index = 0; index < handlers.Count; index++)
            {
                await handlers[index].HandleAsync(integrationEvent, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
