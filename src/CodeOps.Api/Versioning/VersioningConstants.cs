namespace CodeOps.Api.Versioning
{
    /// <summary>
    /// Constants for versioning configuration
    /// </summary>
    public class VersioningConstants
    {
        /// <summary>
        /// Key of header that contains version info
        /// </summary>
        public static readonly string HeaderKey = "x-api-version";

        /// <summary>
        /// Key of query param that contains version info
        /// </summary>
        public static readonly string QueryParameterKey = "api-version";

        /// <summary>
        /// Versioning api explorer group name format
        /// </summary>
        public static readonly string GroupNameFormat = "'v'VVV";

        /// <summary>
        /// Regex to check valid version pattern ex: 1.0, 2.1 etc.
        /// </summary>
        public const string VersionRegex = @"^[0-9].[0-9]$";
    }
}
