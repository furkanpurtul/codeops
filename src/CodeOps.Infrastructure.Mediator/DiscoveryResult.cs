namespace CodeOps.Infrastructure.Mediator
{
    internal sealed record DiscoveryResult
    (
        IReadOnlyCollection<ServiceRegistration> RequestHandlers, 
        IReadOnlyCollection<ServiceRegistration> NotificationHandlers, 
        IReadOnlyCollection<ServiceRegistration> Behaviors
    );
}
