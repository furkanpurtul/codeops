using Asp.Versioning;

using CodeOps.Api.OpenApi;
using CodeOps.Hosting;

namespace CodeOps.Api.Versioning
{
    /// <summary>
    /// Extension methods for api versioning and api explorer
    /// </summary>
    public static partial class DependencyInjection
    {
        /// <summary>
        /// Adds api versioning and api explorer
        /// </summary>
        /// <param name="builder"></param>
        /// <returns><see cref="IHostApplicationBuilder"/></returns>
        public static IHostApplicationBuilder AddVersioning(this IHostApplicationBuilder builder, Action<VersioningOptions>? configure = null)
        {
            builder.RegisterOptions<VersioningOptions>(VersioningOptions.SectionKey);
            var options = builder.GetOptions(VersioningOptions.SectionKey, configure);
            var openApiSettings = builder.Services
                .BuildServiceProvider()
                .GetService<OpenApiOptions>() ?? new OpenApiOptions();

            var splittedVersion = options.DefaultVersion.Split(".");
            var major = Convert.ToInt32(splittedVersion.ElementAt(0));
            var minor = Convert.ToInt32(splittedVersion.ElementAt(1));

            builder.Services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(major, minor);
                opt.ApiVersionReader = ApiVersionReader.Combine
                (
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader(VersioningConstants.HeaderKey),
                    new QueryStringApiVersionReader(VersioningConstants.QueryParameterKey)
                );
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.ReportApiVersions = true;
            })
            .AddMvc()
            .AddApiExplorer(setup =>
            {
                setup.GroupNameFormat = VersioningConstants.GroupNameFormat;
                setup.SubstituteApiVersionInUrl = true;
            })
            .AddOpenApi(versionedOpenApi => OpenApi.DependencyInjection.ConfigureVersionedOpenApi(versionedOpenApi, openApiSettings));

            return builder;
        }
    }
}
