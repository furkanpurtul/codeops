namespace CodeOps.Hosting
{
    /// <summary>
    /// Thrown when the application is started with an invalid or unsupported environment value.
    /// </summary>
    /// <remarks>
    /// This exception is intended to be used when the runtime environment (e.g., via
    /// <c>ASPNETCORE_ENVIRONMENT</c> / <c>DOTNET_ENVIRONMENT</c>) is not one of the explicitly
    /// allowed environment names.
    /// </remarks>
    [Serializable]
    public class InvalidEnvironmentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidEnvironmentException"/> class.
        /// </summary>
        /// <param name="currentEnvironment">The environment value that was provided at runtime.</param>
        /// <param name="allowedEnvironments">The set of allowed environment values.</param>
        public InvalidEnvironmentException(string currentEnvironment, IEnumerable<string> allowedEnvironments)
            : base($"Environment '{currentEnvironment}' has not been set correctly. Allowed values are: {string.Join(", ", allowedEnvironments)}")
        {
        }
    }
}