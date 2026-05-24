using Asp.Versioning.ApiExplorer;
using Asp.Versioning.OpenApi;

using CodeOps.Hosting;

using Microsoft.OpenApi;

namespace CodeOps.Api.OpenApi
{
    /// <summary>
    /// Extension methods for versioned OpenAPI registration.
    /// </summary>
    public static class DependencyInjection
    {
        private const string DeprecatedNotice = "This API version has been deprecated.";

        /// <summary>
        /// Registers OpenAPI settings used by versioned document generation.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configure"></param>
        /// <returns><see cref="IHostApplicationBuilder"/></returns>
        public static IHostApplicationBuilder AddOpenApi(this IHostApplicationBuilder builder, Action<OpenApiOptions>? configure = null)
        {
            builder.RegisterOptions<OpenApiOptions>(OpenApiOptions.SectionKey);
            var settings = builder.GetOptions(OpenApiOptions.SectionKey, configure);

            builder.Services.AddSingleton(settings);

            return builder;
        }

        /// <summary>
        /// Maps versioned OpenAPI json endpoints.
        /// </summary>
        /// <param name="app"></param>
        /// <returns><see cref="WebApplication"/></returns>
        public static WebApplication MapVersionedOpenApi(this WebApplication app)
        {
            app.MapOpenApi().WithDocumentPerVersion();

            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "swagger";
                options.DisplayOperationId();
                options.DisplayRequestDuration();
                options.EnableDeepLinking();
                options.EnablePersistAuthorization();

                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint($"/openapi/{description.GroupName}.json", description.GroupName.ToUpperInvariant());
            });

            return app;
        }

        /// <summary>
        /// Applies version-specific OpenAPI document customizations.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="settings"></param>
        public static void ConfigureVersionedOpenApi(VersionedOpenApiOptions options, OpenApiOptions settings)
        {
            options.Document.AddDocumentTransformer((document, _, cancellationToken) =>
            {
                document.Info = CreateDocumentInfo(options.Description, settings);

                var serverUris = ResolveServerUris(settings);
                if (serverUris.Count > 0)
                    document.Servers = [.. serverUris.Select(uri => new OpenApiServer { Url = uri })];

                return Task.CompletedTask;
            });
        }

        private static OpenApiInfo CreateDocumentInfo(ApiVersionDescription description, OpenApiOptions settings)
        {
            settings.Documents.TryGetValue(description.GroupName, out var configuredInfo);

            var documentInfo = new OpenApiInfo
            {
                Title = configuredInfo?.Title ?? AppDomain.CurrentDomain.FriendlyName,
                Version = configuredInfo?.Version ?? description.GroupName,
                Description = configuredInfo?.Description,
                TermsOfService = configuredInfo?.TermsOfService,
                Contact = configuredInfo?.Contact,
                License = configuredInfo?.License,
            };

            if (description.IsDeprecated)
                documentInfo.Description = string.IsNullOrWhiteSpace(documentInfo.Description)
                    ? DeprecatedNotice
                    : $"{documentInfo.Description}{Environment.NewLine}{Environment.NewLine}{DeprecatedNotice}";

            return documentInfo;
        }

        private static List<string> ResolveServerUris(OpenApiOptions settings)
        {
            var serverUris = new List<string>();

#if DEBUG
            var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");

            if (!string.IsNullOrWhiteSpace(urls))
                serverUris.AddRange(urls.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
#endif

            serverUris.AddRange(settings.ServerUris.Where(uri => !string.IsNullOrWhiteSpace(uri)));

            return [.. serverUris.Distinct(StringComparer.OrdinalIgnoreCase)];
        }
    }
}