using System.Reflection;

using CodeOps.Application.Abstractions.Messaging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CodeOps.Infrastructure.Mediator
{
    public static class DependencyInjection
    {
        public static IHostApplicationBuilder AddMediator(this IHostApplicationBuilder builder, Action<MediatorOptions>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(builder);

            var options = new MediatorOptions();
            configure?.Invoke(options);

            ValidateStrategyType(options.DomainEventPublishStrategyType, typeof(IDomainEventPublishStrategy<>));
            ValidateStrategyType(options.IntegrationEventPublishStrategyType, typeof(IIntegrationEventPublishStrategy<>));

            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(static assembly => IsCandidateAssembly(assembly))
                .Distinct()
                .ToArray();

            var discovery = Discover(assemblies);

            ValidateSingleRequestHandlers(discovery.RequestHandlers);

            builder.Services.TryAddScoped<Mediator>();
            builder.Services.TryAddScoped<ISender>(static sp => sp.GetRequiredService<Mediator>());
            builder.Services.TryAddScoped<IPublisher>(static sp => sp.GetRequiredService<Mediator>());

            builder.Services.TryAddScoped(typeof(IDomainEventPublishStrategy<>), options.DomainEventPublishStrategyType);
            builder.Services.TryAddScoped(typeof(IIntegrationEventPublishStrategy<>), options.IntegrationEventPublishStrategyType);

            RegisterRequestHandlers(builder.Services, discovery.RequestHandlers);
            RegisterNotificationHandlers(builder.Services, discovery.NotificationHandlers);
            RegisterBehaviors(builder.Services, discovery.Behaviors);

            return builder;
        }

        private static DiscoveryResult Discover(IEnumerable<Assembly> assemblies)
        {
            var requestHandlers = new List<ServiceRegistration>();
            var notificationHandlers = new List<ServiceRegistration>();
            var behaviors = new List<ServiceRegistration>();

            foreach (var assembly in assemblies)
            {
                foreach (var type in GetLoadableTypes(assembly))
                {
                    if (type.IsAbstract || type.IsInterface)
                        continue;

                    if (type.IsGenericTypeDefinition)
                    {
                        RegisterOpenGenericBehavior(type, behaviors);
                        continue;
                    }

                    foreach (var serviceType in type.GetInterfaces())
                    {
                        if (!serviceType.IsGenericType)
                            continue;

                        var genericTypeDefinition = serviceType.GetGenericTypeDefinition();

                        if (genericTypeDefinition == typeof(ICommandHandler<>)
                            || genericTypeDefinition == typeof(ICommandHandler<,>)
                            || genericTypeDefinition == typeof(IQueryHandler<,>))
                        {
                            requestHandlers.Add(new ServiceRegistration(serviceType, type));
                            continue;
                        }

                        if (genericTypeDefinition == typeof(IDomainEventHandler<>)
                            || genericTypeDefinition == typeof(IIntegrationEventHandler<>))
                        {
                            notificationHandlers.Add(new ServiceRegistration(serviceType, type));
                            continue;
                        }

                        if (genericTypeDefinition == typeof(IPipelineBehavior<,>))
                        {
                            behaviors.Add(new ServiceRegistration(serviceType, type));
                        }
                    }
                }
            }

            return new DiscoveryResult([.. requestHandlers.Distinct()], [.. notificationHandlers.Distinct()], [.. behaviors.Distinct()]);
        }

        private static void RegisterOpenGenericBehavior(Type type, ICollection<ServiceRegistration> behaviors)
        {
            var implementsPipeline = type
                .GetInterfaces()
                .Any(static i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

            if (!implementsPipeline)
                return;

            behaviors.Add(new ServiceRegistration(typeof(IPipelineBehavior<,>), type));
        }

        private static void ValidateSingleRequestHandlers(IEnumerable<ServiceRegistration> requestHandlers)
        {
            var duplicates = requestHandlers
                .GroupBy(static registration => registration.ServiceType)
                .Where(static group => group.Count() > 1)
                .ToArray();

            if (duplicates.Length == 0)
                return;

            var lines = duplicates.Select(group =>
            {
                var implementations = string.Join(", ", group.Select(static item => item.ImplementationType.FullName));

                var line = $"{group.Key.FullName} => [{implementations}]";
                return line;
            });

            throw new InvalidOperationException
            (
                "Mediator request handler registrations must be unique. Duplicate handlers found:"
                + Environment.NewLine
                + string.Join(Environment.NewLine, lines)
            );
        }

        private static void RegisterRequestHandlers(IServiceCollection services, IEnumerable<ServiceRegistration> registrations)
        {
            foreach (var registration in registrations)
            {
                services.TryAddTransient(registration.ServiceType, registration.ImplementationType);
            }
        }

        private static void RegisterNotificationHandlers(IServiceCollection services, IEnumerable<ServiceRegistration> registrations)
        {
            foreach (var registration in registrations)
            {
                var serviceDescriptor = ServiceDescriptor.Transient(registration.ServiceType, registration.ImplementationType);
                services.TryAddEnumerable(serviceDescriptor);
            }
        }

        private static void RegisterBehaviors(IServiceCollection services, IEnumerable<ServiceRegistration> registrations)
        {
            foreach (var registration in registrations)
            {
                var serviceDescriptor = ServiceDescriptor.Transient(registration.ServiceType, registration.ImplementationType);
                services.TryAddEnumerable(serviceDescriptor);
            }
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.Where(static type => type is not null)!;
            }
        }

        private static bool IsCandidateAssembly(Assembly assembly)
        {
            if (assembly.IsDynamic)
                return false;

            var name = assembly.GetName().Name;
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return !name.Equals("System", StringComparison.Ordinal)
                   && !name.Equals("Microsoft", StringComparison.Ordinal)
                   && !name.Equals("netstandard", StringComparison.Ordinal)
                   && !name.StartsWith("System.", StringComparison.Ordinal)
                   && !name.StartsWith("Microsoft.", StringComparison.Ordinal)
                   && !name.StartsWith("xunit", StringComparison.OrdinalIgnoreCase)
                   && !name.StartsWith("testhost", StringComparison.OrdinalIgnoreCase);
        }

        private static void ValidateStrategyType(Type implementationType, Type expectedOpenGenericServiceType)
        {
            ArgumentNullException.ThrowIfNull(implementationType);
            ArgumentNullException.ThrowIfNull(expectedOpenGenericServiceType);

            if (!implementationType.IsGenericTypeDefinition)
            {
                throw new InvalidOperationException($"'{implementationType.FullName}' must be an open generic type definition.");
            }

            var implementedInterfaces = implementationType.GetInterfaces();

            var isValid = implementedInterfaces.Any(interfaceType =>
                interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == expectedOpenGenericServiceType);

            if (!isValid)
            {
                throw new InvalidOperationException(
                    $"'{implementationType.FullName}' must implement '{expectedOpenGenericServiceType.FullName}'.");
            }
        }
    }
}