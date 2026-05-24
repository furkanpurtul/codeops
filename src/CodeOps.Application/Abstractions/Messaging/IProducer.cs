namespace CodeOps.Application.Abstractions.Messaging
{
    public interface IProducer
    {
        Task ProduceAsync<T>(T message) where T : class, IIntegrationEvent;
    }
}
