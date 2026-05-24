namespace CodeOps.Domain.Abstractions.BuildingBlocks
{
    public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>> 
        where TId : class, IStronglyTypedId
    {
        public TId Id { get; }

        public bool IsTransient => Id.IsDefault();

        IStronglyTypedId IEntity.Id => Id;

        protected Entity(TId id)
        {
            ArgumentNullException.ThrowIfNull(id);
            Id = id;
        }

        public bool Equals(Entity<TId>? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return GetType() == other.GetType()
                   && !IsTransient
                   && !other.IsTransient
                   && EqualityComparer<TId>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object? obj)
        {
            return obj is Entity<TId> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return IsTransient
                ? base.GetHashCode()
                : HashCode.Combine(GetType(), Id);
        }

        public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
            Equals(left, right);

        public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
            !Equals(left, right);
    }
}
