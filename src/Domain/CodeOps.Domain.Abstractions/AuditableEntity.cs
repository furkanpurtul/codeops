namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Base type for entities that track auditing metadata (creation and last modification).
    /// </summary>
    /// <typeparam name="TDerived">
    /// Concrete entity type (CRTP pattern) enabling stronger typing and invariant propagation.
    /// </typeparam>
    /// <typeparam name="TId">
    /// Strongly-typed identity type implementing <see cref="IStronglyTypedId"/>.
    /// </typeparam>
    /// <remarks>
    /// Adds audit fields to <see cref="Entity{TDerived,TId}"/> and provides helper methods
    /// to mark creation and modification by an actor. Timestamps are recorded in UTC.
    /// </remarks>
    public abstract class AuditableEntity<TDerived, TId> : Entity<TDerived, TId>, IAuditable
        where TDerived : AuditableEntity<TDerived, TId>
        where TId : class, IStronglyTypedId
    {
        /// <summary>
        /// Gets the UTC timestamp when the entity was created.
        /// </summary>
        public DateTime CreatedOnUtc { get; private set; }

        /// <summary>
        /// Gets the actor identifier who created the entity.
        /// </summary>
        public string? CreatedBy { get; private set; }

        /// <summary>
        /// Gets the UTC timestamp when the entity was last modified; <c>null</c> if never modified.
        /// </summary>
        public DateTime? LastModifiedOnUtc { get; private set; }

        /// <summary>
        /// Gets the actor identifier who last modified the entity; <c>null</c> if never modified.
        /// </summary>
        public string? LastModifiedBy { get; private set; }

        /// <summary>
        /// Initializes a new auditable entity with identity and default audit values.
        /// </summary>
        /// <param name="id">Strongly-typed entity identity.</param>
        /// <remarks>
        /// Sets <see cref="CreatedOnUtc"/> to <see cref="DateTime.UtcNow"/>. No invariants are passed by default.
        /// </remarks>
        protected AuditableEntity(TId id) : base(id, [])
        {
            CreatedOnUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the entity as created by the specified actor and stamps the current UTC time.
        /// </summary>
        /// <param name="actor">Identifier of the actor that performed the creation.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="actor"/> is <c>null</c>, empty, or whitespace.
        /// </exception>
        public void MarkCreated(string actor)
        {
            if (string.IsNullOrWhiteSpace(actor))
                throw new ArgumentException("Actor cannot be null or whitespace.", nameof(actor));

            CreatedBy = actor;
            CreatedOnUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the entity as modified by the specified actor and stamps the current UTC time.
        /// </summary>
        /// <param name="actor">Identifier of the actor that performed the modification.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="actor"/> is <c>null</c>, empty, or whitespace.
        /// </exception>
        public void MarkModified(string actor)
        {
            if (string.IsNullOrWhiteSpace(actor))
                throw new ArgumentException("Actor cannot be null or whitespace.", nameof(actor));

            LastModifiedBy = actor;
            LastModifiedOnUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Returns a string representation including audit metadata.
        /// </summary>
        public override string ToString() =>
            $"{base.ToString()} [Created: {CreatedOnUtc:O}, CreatedBy: {CreatedBy}, LastModified: {LastModifiedOnUtc:O}, LastModifiedBy: {LastModifiedBy}]";
    }
}
