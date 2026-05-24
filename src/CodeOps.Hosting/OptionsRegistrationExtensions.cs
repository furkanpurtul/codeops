using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace CodeOps.Hosting
{
    /// <summary>
    /// Provides extension methods for registering, retrieving, and validating strongly-typed options
    /// in an <see cref="IHostApplicationBuilder"/> during application startup.
    /// </summary>
    public static class OptionsRegistrationExtensions
    {
        public static IHostApplicationBuilder RegisterOptions<TOptions>(this IHostApplicationBuilder builder, string sectionName)
            where TOptions : class, new()
        {
            var section = builder.Configuration.GetSection(sectionName);
            builder.Services
                .AddOptions<TOptions>()
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return builder;
        }

        public static IHostApplicationBuilder RegisterOptions<TOptions, TValidator>(this IHostApplicationBuilder builder, string sectionName)
            where TOptions : class, new()
            where TValidator : class, IValidateOptions<TOptions>
        {
            builder.Services.AddTransient<IValidateOptions<TOptions>, TValidator>();

            var section = builder.Configuration.GetSection(sectionName);
            builder.Services
                .AddOptions<TOptions>()
                .Bind(section)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return builder;
        }

        public static TOptions GetOptions<TOptions>(this IHostApplicationBuilder builder, string sectionName, Action<TOptions>? configure = null)
            where TOptions : class, new()
        {
            var options = builder.Configuration
                .GetSection(sectionName)
                .Get<TOptions>();

            options ??= new TOptions();

            configure?.Invoke(options);

            return options;
        }
    }
}