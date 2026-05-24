using Microsoft.OpenApi;

namespace CodeOps.Api.OpenApi
{
    /// <summary>
    /// Options used to customize generated OpenAPI documents.
    /// </summary>
    public class OpenApiOptions
    {
        /// <summary>
        /// Key in configuration section.
        /// </summary>
        public static readonly string SectionKey = "OpenApi";

        /// <summary>
        /// Server urls emitted into generated OpenAPI documents.
        /// </summary>
        public List<string> ServerUris { get; set; } = [];

        /// <summary>
        /// Per-version OpenAPI document metadata keyed by group name such as v1.
        /// </summary>
        public Dictionary<string, OpenApiInfo> Documents { get; set; } = [];
    }
}