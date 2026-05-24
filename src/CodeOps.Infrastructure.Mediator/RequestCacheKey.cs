namespace CodeOps.Infrastructure.Mediator
{
    internal readonly record struct RequestCacheKey(RequestKind Kind, Type RequestType, Type ResponseType);
}
