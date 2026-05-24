using System.Text.Json;

using CodeOps.Infrastructure.EntityFrameworkCore.Npgsql;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CodeOps.Api.HealthChecks
{
    /// <summary>
    /// Extension methods for API health checks.
    /// </summary>
    public static class DependencyInjection
    {
        private const string LivenessTag = "liveness";
        private const string ReadinessTag = "readiness";

        /// <summary>
        /// Registers API health checks.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns><see cref="IHostApplicationBuilder"/></returns>
        public static IHostApplicationBuilder AddHealthChecks(this IHostApplicationBuilder builder)
        {
            builder.Services
                .AddHealthChecks()
                .AddCheck("liveness", () => HealthCheckResult.Healthy(), [LivenessTag])
                .AddDbContextCheck<ApplicationDbContext>("postgresql", tags: [ReadinessTag]);

            return builder;
        }

        /// <summary>
        /// Maps health check endpoints.
        /// </summary>
        /// <param name="app"></param>
        /// <returns><see cref="WebApplication"/></returns>
        public static WebApplication MapHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains(LivenessTag),
                ResponseWriter = WriteResponseAsync,
            });

            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = registration => registration.Tags.Contains(ReadinessTag),
                ResponseWriter = WriteResponseAsync,
            });

            return app;
        }

        private static Task WriteResponseAsync(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var payload = new
            {
                status = report.Status.ToString(),
                totalDuration = report.TotalDuration,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    duration = entry.Value.Duration,
                    description = entry.Value.Description,
                }),
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}