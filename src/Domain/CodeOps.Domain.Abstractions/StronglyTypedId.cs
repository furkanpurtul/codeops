namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Base record for strongly typed identifier value objects.
    /// </summary>
    /// <typeparam name="TValue">Underlying raw value type (e.g. <see cref="Guid"/>, <see cref="int"/>, <see cref="string"/>).</typeparam>
    /// <remarks>
    /// Inherit and expose a public constructor that forwards to the protected one to create specific identifier types.
    /// Equality includes both runtime type and underlying value to prevent accidental cross-type equality.
    /// </remarks>
    public abstract record StronglyTypedId<TValue> : IStronglyTypedId, IEquatable<StronglyTypedId<TValue>>
        where TValue : notnull
    {
        /// <summary>
        /// Underlying raw value.
        /// </summary>
        public TValue Value { get; }

        object IStronglyTypedId.Value => Value; 

        /// <summary>
        /// Creates a new strongly typed identifier instance.
        /// </summary>
        /// <param name="value">Raw value to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        protected StronglyTypedId(TValue value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Returns true when the underlying value equals its default value.
        /// </summary>
        public bool IsDefault() => EqualityComparer<TValue>.Default.Equals(Value, default);

        /// <inheritdoc />
        public override string ToString() => Value?.ToString() ?? string.Empty;

        /// <inheritdoc />
        public virtual bool Equals(StronglyTypedId<TValue>? other)
            => other is not null &&
               GetType() == other.GetType() &&
               EqualityComparer<TValue>.Default.Equals(Value, other.Value);

        /// <inheritdoc />
        public override int GetHashCode()
            => HashCode.Combine(GetType(), Value);

        /// <summary>
        /// Implicitly converts the strongly typed identifier to its underlying raw value.
        /// </summary>
        /// <param name="id">Identifier instance.</param>
        public static implicit operator TValue(StronglyTypedId<TValue> id) => id.Value;
    }
}
