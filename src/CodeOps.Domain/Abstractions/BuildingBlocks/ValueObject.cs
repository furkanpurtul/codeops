namespace CodeOps.Domain.Abstractions.BuildingBlocks
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public bool Equals(ValueObject? other)
        {
            return other is not null
                && GetType() == other.GetType()
                && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override bool Equals(object? obj)
        {
            return obj is ValueObject other && Equals(other);
        }

        public override int GetHashCode()
        {
            HashCode hash = default;
            hash.Add(GetType());

            foreach (object? component in GetEqualityComponents())
            {
                hash.Add(component);
            }

            return hash.ToHashCode();
        }

        public static bool operator ==(ValueObject? left, ValueObject? right) =>
            Equals(left, right);

        public static bool operator !=(ValueObject? left, ValueObject? right) =>
            !Equals(left, right);
    }
}
