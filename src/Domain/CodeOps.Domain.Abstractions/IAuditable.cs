namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Contract for audit metadata on domain objects, including creation and last modification details.
    /// </summary>
    /// <remarks>
    /// Timestamps are expressed in UTC. Implementations should validate actor inputs and update audit fields atomically.
    /// </remarks>
    public interface IAuditable
    {
        /// <summary>
        /// Gets the UTC timestamp when the object was created.
        /// </summary>
        DateTime CreatedOnUtc { get; }

        /// <summary>
        /// Gets the identifier of the actor who created the object; <c>null</c> if unknown.
        /// </summary>
        string? CreatedBy { get; }

        /// <summary>
        /// Gets the UTC timestamp when the object was last modified; <c>null</c> if never modified.
        /// </summary>
        DateTime? LastModifiedOnUtc { get; }

        /// <summary>
        /// Gets the identifier of the actor who last modified the object; <c>null</c> if never modified.
        /// </summary>
        string? LastModifiedBy { get; }

        /// <summary>
        /// Marks the object as created by the specified actor and stamps the current UTC time.
        /// </summary>
        /// <param name="actor">Identifier of the actor that performed the creation.</param>
        void MarkCreated(string actor);

        /// <summary>
        /// Marks the object as modified by the specified actor and stamps the current UTC time.
        /// </summary>
        /// <param name="actor">Identifier of the actor that performed the modification.</param>
        void MarkModified(string actor);
    }
}
