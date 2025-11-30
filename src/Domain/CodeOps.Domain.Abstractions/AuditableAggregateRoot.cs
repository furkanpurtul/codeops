namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Base type for aggregate roots that track auditing metadata (creation and last modification).
    /// </summary>
    /// <typeparam name="TDerived">
    /// Concrete aggregate type (CRTP pattern) for stronger typing and invariant propagation.
    /// </typeparam>
    /// <typeparam name="TId">
    /// Strongly-typed identity type implementing <see cref="IStronglyTypedId"/>.
    /// </typeparam>
    /// <remarks>
    /// Adds audit fields to <see cref="AggregateRoot{TDerived,TId}"/> and provides helper methods
    /// to mark creation and modification by an actor. Timestamps are recorded in UTC.
    /// </remarks>
    public abstract class AuditableAggregateRoot<TDerived, TId> : AggregateRoot<TDerived, TId>, IAuditable
        where TDerived : AuditableAggregateRoot<TDerived, TId>
        where TId : class, IStronglyTypedId
    {
        /// <summary>
        /// Gets the UTC timestamp when the aggregate was created.
        /// </summary>
        public DateTime CreatedOnUtc { get; private set; }

        /// <summary>
        /// Gets the actor identifier who created the aggregate.
        /// </summary>
        public string? CreatedBy { get; private set; }

        /// <summary>
        /// Gets the UTC timestamp when the aggregate was last modified; <c>null</c> if never modified.
        /// </summary>
        public DateTime? LastModifiedOnUtc { get; private set; }

        /// <summary>
        /// Gets the actor identifier who last modified the aggregate; <c>null</c> if never modified.
        /// </summary>
        public string? LastModifiedBy { get; private set; }

        /// <summary>
        /// Initializes a new auditable aggregate root with identity and default audit values.
        /// </summary>
        /// <param name="id">Strongly-typed aggregate identity.</param>
        /// <remarks>
        /// Sets <see cref="CreatedOnUtc"/> to <see cref="DateTime.UtcNow"/>. No invariants are passed by default.
        /// </remarks>
        protected AuditableAggregateRoot(TId id) : base(id, [])
        {
            CreatedOnUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Marks the aggregate as created by the specified actor and stamps the current UTC time.
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
        /// Marks the aggregate as modified by the specified actor and stamps the current UTC time.
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
