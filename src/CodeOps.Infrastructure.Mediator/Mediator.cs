using System.Collections.Concurrent;
using System.Reflection;

using CodeOps.Application.Abstractions.Messaging;
using CodeOps.Domain.Abstractions.BuildingBlocks;

using Microsoft.Extensions.DependencyInjection;

namespace CodeOps.Infrastructure.Mediator
{
    internal sealed class Mediator : ISender, IPublisher
    {
        private static readonly ConcurrentDictionary<RequestCacheKey, RequestInvoker> _requestInvokers = new();
        private static readonly ConcurrentDictionary<PublishCacheKey, PublishInvoker> _publishInvokers = new();
        private static readonly ConcurrentDictionary<Type, int> _behaviorOrders = new();

        private static readonly MethodInfo _sendCommandVoidMethod = GetRequiredMethod(nameof(SendCommandVoidAsync));
        private static readonly MethodInfo _sendCommandWithResponseMethod = GetRequiredMethod(nameof(SendCommandWithResponseAsync));
        private static readonly MethodInfo _sendQueryMethod = GetRequiredMethod(nameof(SendQueryAsync));
        private static readonly MethodInfo _publishDomainEventMethod = GetRequiredMethod(nameof(PublishDomainEventAsync));
        private static readonly MethodInfo _publishIntegrationEventMethod = GetRequiredMethod(nameof(PublishIntegrationEventAsync));

        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            
            _serviceProvider = serviceProvider;
        }

        public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var cacheKey = new RequestCacheKey(RequestKind.CommandWithoutResponse, command.GetType(), typeof(Unit));
            var invoker = _requestInvokers.GetOrAdd(cacheKey, static key => CreateRequestInvoker(key));

            _ = await invoker(_serviceProvider, command, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var cacheKey = new RequestCacheKey(RequestKind.CommandWithResponse, command.GetType(), typeof(TResponse));
            var invoker = _requestInvokers.GetOrAdd(cacheKey, static key => CreateRequestInvoker(key));

            var result = await invoker(_serviceProvider, command, cancellationToken).ConfigureAwait(false);

            return (TResponse)result!;
        }

        public async Task<TResponse> QueryAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(query);

            var cacheKey = new RequestCacheKey(RequestKind.Query, query.GetType(), typeof(TResponse));
            var invoker = _requestInvokers.GetOrAdd(cacheKey, static key => CreateRequestInvoker(key));

            var result = await invoker(_serviceProvider, query, cancellationToken).ConfigureAwait(false);
            return (TResponse)result!;
        }

        public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(@event);

            var cacheKey = new PublishCacheKey(PublishKind.DomainEvent, @event.GetType());

            var invoker = _publishInvokers.GetOrAdd(cacheKey, static key => CreatePublishInvoker(key));

            return invoker(_serviceProvider, @event, cancellationToken);
        }

        public Task PublishAsync(IIntegrationEvent @event, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(@event);

            var cacheKey = new PublishCacheKey(PublishKind.IntegrationEvent, @event.GetType());

            var invoker = _publishInvokers.GetOrAdd(cacheKey, static key => CreatePublishInvoker(key));

            return invoker(_serviceProvider, @event, cancellationToken);
        }

        private static RequestInvoker CreateRequestInvoker(RequestCacheKey key)
        {
            var closedMethod = key.Kind switch
            {
                RequestKind.CommandWithoutResponse => _sendCommandVoidMethod.MakeGenericMethod(key.RequestType),
                RequestKind.CommandWithResponse => _sendCommandWithResponseMethod.MakeGenericMethod(key.RequestType, key.ResponseType),
                RequestKind.Query => _sendQueryMethod.MakeGenericMethod(key.RequestType, key.ResponseType),
                _ => throw new InvalidOperationException($"Unsupported request kind: {key.Kind}.")
            };

            return closedMethod.CreateDelegate<RequestInvoker>();
        }

        private static PublishInvoker CreatePublishInvoker(PublishCacheKey key)
        {
            MethodInfo closedMethod = key.Kind switch
            {
                PublishKind.DomainEvent => _publishDomainEventMethod.MakeGenericMethod(key.EventType),
                PublishKind.IntegrationEvent => _publishIntegrationEventMethod.MakeGenericMethod(key.EventType),
                _ => throw new InvalidOperationException($"Unsupported publish kind: {key.Kind}.")
            };

            return closedMethod.CreateDelegate<PublishInvoker>();
        }

        private static async Task<object?> SendCommandVoidAsync<TCommand>(IServiceProvider serviceProvider, object request, CancellationToken cancellationToken)
            where TCommand : class, ICommand
        {
            var command = (TCommand)request;
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();

            async Task<Unit> HandlerDelegate()
            {
                await handler.HandleAsync(command, cancellationToken).ConfigureAwait(false);
                return Unit.Value;
            }

            var pipeline = BuildPipeline(serviceProvider, command, HandlerDelegate, cancellationToken);
            _ = await pipeline().ConfigureAwait(false);

            return null;
        }

        private static async Task<object?> SendCommandWithResponseAsync<TCommand, TResponse>(IServiceProvider serviceProvider, object request, CancellationToken cancellationToken)
            where TCommand : class, ICommand<TResponse>
        {
            var command = (TCommand)request;
            var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

            Task<TResponse> HandlerDelegate() => handler.HandleAsync(command, cancellationToken);

            var pipeline = BuildPipeline(serviceProvider, command, HandlerDelegate, cancellationToken);
            var result = await pipeline().ConfigureAwait(false);

            return result;
        }

        private static async Task<object?> SendQueryAsync<TQuery, TResponse>(IServiceProvider serviceProvider, object request, CancellationToken cancellationToken)
            where TQuery : class, IQuery<TResponse>
        {
            var query = (TQuery)request;
            var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

            Task<TResponse> HandlerDelegate() => handler.HandleAsync(query, cancellationToken);

            var pipeline = BuildPipeline(serviceProvider, query, HandlerDelegate, cancellationToken);
            var result = await pipeline().ConfigureAwait(false);

            return result;
        }

        private static async Task PublishDomainEventAsync<TEvent>(IServiceProvider serviceProvider, object @event, CancellationToken cancellationToken)
            where TEvent : class, IDomainEvent
        {
            var domainEvent = (TEvent)@event;
            var handlers = serviceProvider.GetServices<IDomainEventHandler<TEvent>>().ToArray();
            var strategy = serviceProvider.GetRequiredService<IDomainEventPublishStrategy<TEvent>>();

            await strategy.PublishAsync(handlers, domainEvent, cancellationToken).ConfigureAwait(false);
        }

        private static async Task PublishIntegrationEventAsync<TEvent>(IServiceProvider serviceProvider, object @event, CancellationToken cancellationToken)
            where TEvent : class, IIntegrationEvent
        {
            var integrationEvent = (TEvent)@event;
            var handlers = serviceProvider.GetServices<IIntegrationEventHandler<TEvent>>().ToArray();
            var strategy = serviceProvider.GetRequiredService<IIntegrationEventPublishStrategy<TEvent>>();

            await strategy.PublishAsync(handlers, integrationEvent, cancellationToken).ConfigureAwait(false);
        }

        private static RequestHandlerDelegate<TResponse> BuildPipeline<TRequest, TResponse>
        (
            IServiceProvider serviceProvider,
            TRequest request,
            RequestHandlerDelegate<TResponse> handlerDelegate,
            CancellationToken cancellationToken
        ) where TRequest : class
        {
            var behaviors = serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .OrderBy(static behavior => GetBehaviorOrder(behavior.GetType()))
                .ToArray();

            RequestHandlerDelegate<TResponse> pipeline = handlerDelegate;

            for (var index = behaviors.Length - 1; index >= 0; index--)
            {
                var behavior = behaviors[index];
                var next = pipeline;
                pipeline = () => behavior.HandleAsync(request, next, cancellationToken);
            }

            return pipeline;
        }

        private static int GetBehaviorOrder(Type behaviorType)
        {
            return _behaviorOrders.GetOrAdd(behaviorType, static type =>
            {
                var attribute = type.GetCustomAttribute<BehaviorOrderAttribute>();
                return attribute?.Order ?? 0;
            });
        }

        private static MethodInfo GetRequiredMethod(string methodName)
        {
            var method = typeof(Mediator).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic)
                ?? throw new InvalidOperationException($"Method '{methodName}' was not found.");
            return method;
        }

        private delegate Task<object?> RequestInvoker(IServiceProvider serviceProvider, object request, CancellationToken cancellationToken);

        private delegate Task PublishInvoker(IServiceProvider serviceProvider, object notification, CancellationToken cancellationToken);
    }
}