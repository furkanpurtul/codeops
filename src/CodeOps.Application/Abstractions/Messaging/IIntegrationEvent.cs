namespace CodeOps.Application.Abstractions.Messaging
{
    public interface IIntegrationEvent
    {
        public string? PartitionKey { get; init; }
    }
}
