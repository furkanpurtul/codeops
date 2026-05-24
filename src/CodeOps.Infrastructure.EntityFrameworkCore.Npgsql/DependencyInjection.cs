using CodeOps.Application.Abstractions.Persistence;
using CodeOps.Hosting;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CodeOps.Infrastructure.EntityFrameworkCore.Npgsql
{
    public static class DependencyInjection
    {
        public static IHostApplicationBuilder AddNpgsql(this IHostApplicationBuilder builder, Action<NpgsqlOptions>? configure = null)
        {
            builder.RegisterOptions<NpgsqlOptions>(NpgsqlOptions.SectionKey);

            var npgsqlOptions = builder.GetOptions(NpgsqlOptions.SectionKey, configure);

            builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
            {
#if DEBUG
                npgsqlOptions.Host = "localhost";
                optionsBuilder.UseNpgsql(npgsqlOptions.ConnectionString);
                optionsBuilder.EnableDetailedErrors();
                optionsBuilder.EnableSensitiveDataLogging();
#else
                var monitor = serviceProvider.GetRequiredService<IOptionsMonitor<NpgsqlOptions>>();
                optionsBuilder.UseNpgsql(monitor.CurrentValue.ConnectionString);
#endif
            });

            builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

            return builder;
        }
    }
}
