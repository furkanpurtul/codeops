using System.Runtime.CompilerServices;

namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Base type for domain entities providing identity, transient detection, validation support,
    /// and value-based equality semantics using strongly-typed identifiers.
    /// </summary>
    /// <typeparam name="TDerived">
    /// Concrete entity type (CRTP pattern) enabling strongly-typed validation and equality.
    /// </typeparam>
    /// <typeparam name="TId">
    /// Strongly-typed identity type implementing <see cref="IStronglyTypedId"/>.
    /// </typeparam>
    public abstract class Entity<TDerived, TId> : Validatable<TDerived>, IEntity<TId>, IEquatable<TDerived>
        where TDerived : Entity<TDerived, TId>
        where TId : class, IStronglyTypedId
    {
        /// <summary>
        /// Gets the strongly-typed identity of the entity.
        /// </summary>
        public TId Id { get; }

        /// <summary>
        /// Gets a value indicating whether the entity has not yet been assigned a persistent identity.
        /// </summary>
        /// <remarks>
        /// Delegates to <see cref="IStronglyTypedId.IsDefault"/> on the underlying id value.
        /// </remarks>
        public bool IsTransient => Id.IsDefault();

        /// <summary>
        /// Gets the identity as <see cref="IStronglyTypedId"/> for non-generic consumers.
        /// </summary>
        IStronglyTypedId IEntity.Id => Id;

        /// <summary>
        /// Initializes a new entity with identity and invariants.
        /// </summary>
        /// <param name="id">Strongly-typed entity identity.</param>
        /// <param name="invariants">Invariant rules enforced by the <see cref="Validatable{T}"/> base.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is null.</exception>
        protected Entity(TId id, IRule<TDerived>[] invariants) : base(invariants)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        /// <summary>
        /// Returns the runtime type excluding potential proxy overlays.
        /// </summary>
        /// <remarks>
        /// Override in derived types when using proxying frameworks (e.g., lazy loading) to ensure
        /// equality and hashing are based on the actual unproxied type.
        /// </remarks>
        protected virtual Type GetUnproxiedType() => GetType();

        /// <summary>
        /// Determines whether the specified object is equal to the current entity.
        /// </summary>
        /// <param name="obj">The object to compare with the current entity.</param>
        /// <returns><c>true</c> if the objects are equal; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Uses reference equality first, then delegates to <see cref="Equals(TDerived)"/> for typed comparison.
        /// </remarks>
        public sealed override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is TDerived derived) return Equals(derived);
            return false;
        }

        /// <summary>
        /// Determines whether the specified entity is equal to the current entity.
        /// </summary>
        /// <param name="other">The other entity instance.</param>
        /// <returns><c>true</c> if the entities are equal; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Equality requires:
        /// 1. Same unproxied runtime type.
        /// 2. Both entities are non-transient.
        /// 3. Equal strongly-typed id values.
        /// </remarks>
        public bool Equals(TDerived? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            var thisType = GetUnproxiedType();
            var otherType = other.GetUnproxiedType();
            if (thisType != otherType) return false;

            if (IsTransient || other.IsTransient) return false;

            return Equals(Id.Value, other.Id.Value);
        }

        /// <summary>
        /// Returns a hash code for the entity.
        /// </summary>
        /// <returns>A hash code suitable for hashing algorithms and data structures.</returns>
        /// <remarks>
        /// Transient entities use identity-based hashing via <see cref="RuntimeHelpers.GetHashCode(object)"/>.
        /// Persisted entities combine unproxied type and id value to avoid collisions across types.
        /// </remarks>
        public override int GetHashCode()
        {
            if (IsTransient)
            {
                return RuntimeHelpers.GetHashCode(this);
            }

            return HashCode.Combine(GetUnproxiedType(), Id.Value);
        }

        /// <summary>
        /// Determines whether two entities are equal.
        /// </summary>
        /// <param name="left">The left entity.</param>
        /// <param name="right">The right entity.</param>
        /// <returns><c>true</c> if equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(Entity<TDerived, TId>? left, Entity<TDerived, TId>? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals((TDerived)right);
        }

        /// <summary>
        /// Determines whether two entities are not equal.
        /// </summary>
        /// <param name="left">The left entity.</param>
        /// <param name="right">The right entity.</param>
        /// <returns><c>true</c> if not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(Entity<TDerived, TId>? left, Entity<TDerived, TId>? right) => !(left == right);
    }
}
