namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Framework marker interface for strongly typed identifier value objects.
    /// </summary>
    /// <remarks>
    /// Implementations wrap a primitive (guid, int, string, etc.) to provide type safety across boundaries.
    /// </remarks>
    public interface IStronglyTypedId
    {
        /// <summary>
        /// Underlying raw value of the identifier.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Indicates whether the underlying value equals its default value.
        /// </summary>
        bool IsDefault();
    }
}
