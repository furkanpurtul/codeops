using CodeOps.Application.Abstractions.Messaging;

namespace CodeOps.Infrastructure.Mediator
{
    internal interface IIntegrationEventPublishStrategy<TEvent> where TEvent : class, IIntegrationEvent
    {
        Task PublishAsync(IReadOnlyList<IIntegrationEventHandler<TEvent>> handlers, TEvent integrationEvent, CancellationToken cancellationToken);
    }
}
