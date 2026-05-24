using CodeOps.Domain.Abstractions.BuildingBlocks;

namespace CodeOps.Application.Abstractions.Messaging
{
    public interface IPublisher
    {
        Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default);

        Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default);
    }
}