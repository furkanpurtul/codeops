using System.ComponentModel.DataAnnotations;

namespace CodeOps.Api.Versioning
{
    /// <summary>
    /// Versioning options
    /// </summary>
    public class VersioningOptions
    {
        /// <summary>
        /// Key in configuration section
        /// </summary>
        public static readonly string SectionKey = "Versioning";

        /// <summary>
        /// Sets default version
        /// </summary>
        [RegularExpression(VersioningConstants.VersionRegex)]
        public string DefaultVersion { get; set; } = "1.0";
    }
}
