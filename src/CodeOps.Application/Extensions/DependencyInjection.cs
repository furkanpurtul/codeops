using Microsoft.Extensions.Hosting;

namespace CodeOps.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IHostApplicationBuilder AddApplication(this IHostApplicationBuilder builder)
        {

            return builder;
        }
    }
}
