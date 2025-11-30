namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Marker abstraction for repositories. Extend with data operation methods if needed.
    /// </summary>
    public interface IRepository<TAggregateRoot>
        where TAggregateRoot : IAggregateRoot
    {
    }
}

