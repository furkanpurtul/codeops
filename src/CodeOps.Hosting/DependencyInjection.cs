using Microsoft.Extensions.Hosting;

namespace CodeOps.Hosting
{
    /// <summary>
    /// Extension methods for <see cref="IHostApplicationBuilder"/>
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Validates environment
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnvironmentException"></exception>
        public static IHostApplicationBuilder AddHosting(this IHostApplicationBuilder builder)
        {
            var env = builder.Environment.EnvironmentName;
            var isValidEnvironment = Environments.IsValid(env);
            if (!isValidEnvironment)
            {
                var allowedEnvs = Environments.GetEnvironments();
                throw new InvalidEnvironmentException(env, allowedEnvs);
            }

            return builder;
        }
    }
}
