namespace CodeOps.Infrastructure.Mediator
{
    public sealed class MediatorOptions
    {
        public Type DomainEventPublishStrategyType { get; set; } = typeof(SequentialDomainEventPublishStrategy<>);

        public Type IntegrationEventPublishStrategyType { get; set; } = typeof(SequentialIntegrationEventPublishStrategy<>);
    }
}
