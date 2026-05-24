using System.Reflection;

namespace CodeOps.Hosting
{
    /// <summary>
    /// Environment names to be used
    /// </summary>
    public class Environments
    {
        /// <summary>
        /// Specifies the Development environment.
        /// </summary>
        /// <remarks>The development environment can enable features that shouldn't be exposed in production. Because of the performance cost, scope validation and dependency validation only happens in development.</remarks>
        public static readonly string Development = "Development";

        /// <summary>
        /// Specifies the Test environment.
        /// </summary>
        /// <remarks>The development environment to be used to test app chnages.</remarks>
        public static readonly string Test = "Test";

        /// <summary>
        /// Specifies the Acceptance environment.
        /// </summary>
        /// <remarks>The acceptance environment can be used to validate app changes before changing the environment to production.</remarks>
        public static readonly string Staging = "Staging";

        /// <summary>
        /// Specifies the Production environment.
        /// </summary>
        /// <remarks>The production environment should be configured to maximize security, performance, and application robustness.</remarks>
        public static readonly string Production = "Production";

        /// <summary>
        /// Checks the environment is valid
        /// </summary>
        /// <param name="environment"></param>
        /// <returns>True if matches with any host environment <see cref="Environments"/></returns>
        public static bool IsValid(string environment)
        {
            return GetEnvironments().Any(env => string.Equals(environment, env));
        }

        /// <summary>
        /// Retrieves all public static string fields defined in the HostingEnvironment type.
        /// </summary>
        /// <returns>A read-only list of environment names.</returns>
        public static IReadOnlyList<string> GetEnvironments()
        {
            return typeof(Environments)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(string))
                .Select(f => (string)f.GetValue(null)!)
                .ToList()
                .AsReadOnly();
        }
    }
}