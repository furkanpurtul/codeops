using CodeOps.Application.Abstractions.Messaging;

namespace CodeOps.Infrastructure.Mediator
{
    internal sealed class ParallelWhenAllIntegrationEventPublishStrategy<TEvent> : IIntegrationEventPublishStrategy<TEvent>
        where TEvent : class, IIntegrationEvent
    {
        public Task PublishAsync(IReadOnlyList<IIntegrationEventHandler<TEvent>> handlers, TEvent integrationEvent, CancellationToken cancellationToken)
        {
            return handlers.Count switch
            {
                0 => Task.CompletedTask,
                1 => handlers[0].HandleAsync(integrationEvent, cancellationToken),
                _ => PublishWhenAllAsync(handlers, integrationEvent, cancellationToken)
            };
        }

        private static Task PublishWhenAllAsync(IReadOnlyList<IIntegrationEventHandler<TEvent>> handlers, TEvent integrationEvent, CancellationToken cancellationToken)
        {
            var tasks = new Task[handlers.Count];

            for (var index = 0; index < handlers.Count; index++)
            {
                tasks[index] = handlers[index].HandleAsync(integrationEvent, cancellationToken);
            }

            return Task.WhenAll(tasks);
        }
    }
}
