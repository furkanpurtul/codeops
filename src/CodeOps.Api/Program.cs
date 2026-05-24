using System.Text.Json.Serialization;

using CodeOps.Api.HealthChecks;
using CodeOps.Api.OpenApi;
using CodeOps.Api.Versioning;
using CodeOps.Hosting;
using CodeOps.Infrastructure.Mediator;

namespace CodeOps.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.AddObservability();

            // Add services to the container.
            builder.AddHosting();
            builder.AddHealthChecks();
            builder.AddOpenApi();
            builder.AddVersioning();
            builder.AddMediator();

            builder.Services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            builder.Services.AddProblemDetails();

            var app = builder.Build();

            app.MapHealthChecks();

            // Configure the HTTP request pipeline.
            if (!builder.Environment.IsProduction())
            {
                app.MapVersionedOpenApi();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Problem details and status code pages should be the last middlewares in the pipeline to catch all exceptions and status codes
            app.UseExceptionHandler();
            app.UseStatusCodePages();

            app.Run();
        }
    }
}
