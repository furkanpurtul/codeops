namespace CodeOps.Domain.Abstractions.BuildingBlocks
{
    public interface IEntity
    {
        IStronglyTypedId Id { get; }

        bool IsTransient { get; }
    }

    public interface IEntity<out TId> : IEntity where TId : IStronglyTypedId
    {
        new TId Id { get; }
    }
}