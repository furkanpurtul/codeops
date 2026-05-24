namespace CodeOps.Domain.Abstractions.BuildingBlocks
{
    public interface IRepository<TAggregateRoot> where TAggregateRoot : IAggregateRoot;
}
