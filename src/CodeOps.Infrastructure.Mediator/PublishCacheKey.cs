namespace CodeOps.Infrastructure.Mediator
{
    internal readonly record struct PublishCacheKey(PublishKind Kind, Type EventType);
}
