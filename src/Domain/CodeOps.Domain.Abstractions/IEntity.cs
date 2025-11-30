namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Defines a non-generic entity abstraction for infrastructure concerns that require an identity
    /// without depending on the concrete strongly-typed id type.
    /// </summary>
    /// <remarks>
    /// This interface enables handling of entities uniformly when the specific <see cref="IStronglyTypedId"/> type
    /// is either unknown or irrelevant to the current operation (e.g. persistence, auditing, caching).
    /// Use <see cref="IEntity{TId}"/> when you need compile-time access to the concrete id type.
    /// </remarks>
    /// <seealso cref="IStronglyTypedId"/>
    public interface IEntity
    {
        /// <summary>
        /// Gets the identity of the entity as an untyped strongly-typed id.
        /// </summary>
        /// <remarks>
        /// Although the property returns <see cref="IStronglyTypedId"/>, internally the concrete implementation
        /// will usually expose a specific derived strongly-typed id. Use <see cref="IEntity{TId}"/> when you need
        /// the concrete id type directly.
        /// </remarks>
        IStronglyTypedId Id { get; }

        /// <summary>
        /// Gets a value indicating whether the entity has not yet been assigned a persistent identity.
        /// </summary>
        /// <remarks>
        /// Typically, newly constructed domain entities (prior to persistence) are considered transient.
        /// After a persistence operation assigns a durable id, <c>IsTransient</c> should return <c>false</c>.
        /// </remarks>
        bool IsTransient { get; }
    }

    /// <summary>
    /// Defines a strongly-typed entity abstraction exposing the concrete id type.
    /// </summary>
    /// <typeparam name="TId">
    /// The strongly-typed id type implementing <see cref="IStronglyTypedId"/>.
    /// </typeparam>
    /// <remarks>
    /// This generic specialization improves type safety where the concrete identity type is relevant.
    /// It hides the untyped <see cref="IEntity.Id"/> with a covariant, typed version.
    /// </remarks>
    /// <seealso cref="IStronglyTypedId"/>
    public interface IEntity<out TId> : IEntity
        where TId : IStronglyTypedId
    {
        /// <summary>
        /// Gets the strongly-typed identity of the entity.
        /// </summary>
        /// <remarks>
        /// This property shadows <see cref="IEntity.Id"/> with a covariant return of <typeparamref name="TId"/>.
        /// Consumers that only need a non-generic abstraction can treat instances as <see cref="IEntity"/>.
        /// </remarks>
        new TId Id { get; }
    }
}