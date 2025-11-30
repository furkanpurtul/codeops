namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Base class for immutable value objects that implement structural equality and invariant validation.
    /// </summary>
    /// <typeparam name="TDerived">Concrete value object type (CRTP pattern).</typeparam>
    /// <remarks>
    /// <para>
    /// Responsibilities:
    /// <list type="bullet">
    /// <item><description>Provides structural (component-based) equality &amp; hashing.</description></item>
    /// <item><description>Integrates invariant evaluation/enforcement via <see cref="Validatable{TDerived}"/>.</description></item>
    /// <item><description>Supports extensibility for component comparison through <see cref="EqualityComponentComparer"/> or per-component <see cref="ComponentComparers"/>.</description></item>
    /// <item><description>Thread-safe, one-time materialization &amp; caching of equality components to avoid repeated iteration / allocation.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Caching Strategy:
    /// <list type="bullet">
    /// <item><description>The sequence returned by <see cref="GetEqualityComponents"/> is materialized once per instance and stored as an array implementing <see cref="IReadOnlyList{T}"/>.</description></item>
    /// <item><description>Initialization is performed with a lock-free <see cref="Interlocked.CompareExchange{T}(ref T, T, T)"/> to remain thread-safe under concurrent first access.</description></item>
    /// <item><description>Assumes immutability of derived value objects; if mutability is introduced, cached components may become stale.</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Custom Comparison:
    /// <list type="bullet">
    /// <item><description>Override <see cref="EqualityComponentComparer"/> to apply a single comparer to all components.</description></item>
    /// <item><description>Alternatively override <see cref="ComponentComparers"/> to supply a comparer per component (index-aligned). Null entries fall back to <see cref="EqualityComponentComparer"/>.</description></item>
    /// <item><description>If <see cref="ComponentComparers"/> count mismatches component count, the unified comparer path is used.</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <seealso cref="Validatable{TContext}"/>
    public abstract class ValueObject<TDerived> : Validatable<TDerived>, IEquatable<TDerived>
        where TDerived : ValueObject<TDerived>
    {
        // Cached materialized equality components (lazy + thread-safe, one-time).
        private IReadOnlyList<object?>? _equalityComponentsCache; // backing field (array instance)

        /// <summary>
        /// Initializes a new instance of the value object.
        /// </summary>
        /// <param name="invariants">Invariant rules to validate; may be empty or null.</param>
        protected ValueObject(IRule<TDerived>[] invariants) : base(invariants) { }

        /// <summary>
        /// Provides the ordered set of components that participate in equality and hashing.
        /// </summary>
        /// <remarks>
        /// <para>Implementations should yield immutable data to preserve correctness of caching.</para>
        /// <para>Order is significant. Null values are supported.</para>
        /// </remarks>
        /// <returns>Ordered sequence of structural equality components.</returns>
        protected abstract IEnumerable<object?> GetEqualityComponents();

        /// <summary>
        /// Gets the comparer used to evaluate equality between corresponding components (uniform comparer path).
        /// </summary>
        protected virtual IEqualityComparer<object?> EqualityComponentComparer => EqualityComparer<object?>.Default;

        /// <summary>
        /// Optional per-component comparers (index-aligned with components). When provided and the count
        /// matches the component count, each component is compared with its corresponding comparer. Null
        /// entries fall back to <see cref="EqualityComponentComparer"/>. Default is <c>null</c> (disabled).
        /// </summary>
        protected virtual IReadOnlyList<IEqualityComparer<object?>>? ComponentComparers => null;

        /// <summary>
        /// Returns the cached, materialized equality components list, computing it once if necessary (thread-safe, lock-free).
        /// </summary>
        private IReadOnlyList<object?> GetCachedComponents()
        {
            var current = Volatile.Read(ref _equalityComponentsCache);
            if (current is not null) return current;

            var materialized = GetEqualityComponents().ToArray();
            // Attempt one-time publication; if another thread already published, discard local.
            Interlocked.CompareExchange(ref _equalityComponentsCache, materialized, null);
            return _equalityComponentsCache!; // now non-null
        }

        /// <summary>
        /// Strongly typed equality comparison.
        /// </summary>
        /// <param name="other">Other value object.</param>
        /// <returns><c>true</c> if structurally equal; otherwise <c>false</c>.</returns>
        public bool Equals(TDerived? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            var left = GetCachedComponents();
            var right = other.GetCachedComponents();
            if (left.Count != right.Count) return false; // defensive (should not happen)

            var perComponent = ComponentComparers;
            if (perComponent is not null && perComponent.Count == left.Count)
            {
                for (int i = 0; i < left.Count; i++)
                {
                    var cmp = perComponent[i] ?? EqualityComponentComparer;
                    if (!cmp.Equals(left[i], right[i])) return false;
                }
                return true;
            }
            // fallback unified comparer
            return left.SequenceEqual(right, EqualityComponentComparer);
        }

        /// <summary>
        /// Object equality override delegating to typed equality.
        /// </summary>
        public override bool Equals(object? obj) => obj is TDerived other && Equals(other);

        /// <summary>
        /// Computes a hash code combining all equality components.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="HashCode"/> accumulator (improved mixing vs manual prime multiplication).
        /// Per-component comparers (when provided) contribute hashed values using their corresponding comparer.
        /// </remarks>
        public override int GetHashCode()
        {
            var components = GetCachedComponents();
            var perComponent = ComponentComparers;
            var hasPerComponent = perComponent is not null && perComponent.Count == components.Count;
            var fallbackComparer = EqualityComponentComparer;
            var hc = new HashCode();

            for (int i = 0; i < components.Count; i++)
            {
                var value = components[i];
                var cmp = hasPerComponent ? perComponent![i] ?? fallbackComparer : fallbackComparer;
                // HashCode.Add(object?, IEqualityComparer<object?>) uses comparer to compute the value hash
                hc.Add(value, cmp);
            }
            return hc.ToHashCode();
        }

        /// <summary>
        /// Equality operator (null-safe) leveraging structural equality.
        /// </summary>
        public static bool operator ==(ValueObject<TDerived>? left, ValueObject<TDerived>? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator (null-safe).
        /// </summary>
        public static bool operator !=(ValueObject<TDerived>? left, ValueObject<TDerived>? right) => !(left == right);
    }
}
