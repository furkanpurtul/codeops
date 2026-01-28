using System;
using System.Collections.Concurrent;

namespace CodeOps.Domain.Abstractions
{
    /// <summary>
    /// Default implementation of <see cref="IDomainEventDispatcher"/> that uses a service provider
    /// to resolve and invoke domain event handlers.
    /// </summary>
    /// <remarks>
    /// This dispatcher resolves all registered handlers for each event type and invokes them sequentially.
    /// Handler exceptions are collected and wrapped in an <see cref="AggregateException"/> if multiple handlers fail.
    /// Thread-safe for concurrent dispatch operations.
    /// </remarks>
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Type, Type> _handlerTypeCache = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventDispatcher"/> class.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider used to resolve domain event handlers.
        /// Must not be null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="serviceProvider"/> is null.
        /// </exception>
        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc/>
        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);

            var eventType = domainEvent.GetType();
            var handlerType = GetHandlerType(eventType);
            var handlers = GetHandlers(handlerType);

            if (handlers.Count == 0)
            {
                // No handlers registered - this is acceptable, events may be informational
                return;
            }

            var exceptions = new List<Exception>();

            foreach (var handler in handlers)
            {
                try
                {
                    await InvokeHandlerAsync(handler, domainEvent, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(new DomainEventHandlerException(
                        $"Handler {handler.GetType().Name} failed to process event {eventType.Name}",
                        ex));
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(
                    $"One or more handlers failed to process event {eventType.Name}",
                    exceptions);
            }
        }

        /// <inheritdoc/>
        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(domainEvents);

            var eventList = domainEvents.ToList();
            if (eventList.Count == 0)
            {
                return;
            }

            var exceptions = new List<Exception>();

            foreach (var domainEvent in eventList)
            {
                try
                {
                    await DispatchAsync(domainEvent, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(
                    "One or more domain events failed to dispatch",
                    exceptions);
            }
        }

        /// <inheritdoc/>
        public async Task DispatchAsync(IAggregateRoot aggregateRoot, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(aggregateRoot);

            var events = aggregateRoot.DequeueDomainEvents();
            await DispatchAsync(events, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the generic handler type for the specified event type.
        /// </summary>
        /// <param name="eventType">The concrete event type.</param>
        /// <returns>The closed generic type of <see cref="IDomainEventHandler{TEvent}"/>.</returns>
        private Type GetHandlerType(Type eventType)
        {
            return _handlerTypeCache.GetOrAdd(eventType, static type =>
                typeof(IDomainEventHandler<>).MakeGenericType(type));
        }

        /// <summary>
        /// Resolves all handlers for the specified handler type from the service provider.
        /// </summary>
        /// <param name="handlerType">The closed generic handler type.</param>
        /// <returns>A list of handler instances.</returns>
        private List<object> GetHandlers(Type handlerType)
        {
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(handlerType);
            var handlers = _serviceProvider.GetService(enumerableType);

            if (handlers is System.Collections.IEnumerable enumerable)
            {
                return enumerable.Cast<object>().ToList();
            }

            return [];
        }

        /// <summary>
        /// Invokes a handler's HandleAsync method using reflection.
        /// </summary>
        /// <param name="handler">The handler instance.</param>
        /// <param name="domainEvent">The domain event to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task InvokeHandlerAsync(object handler, IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            var handleMethod = handler.GetType().GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
            
            if (handleMethod == null)
            {
                throw new InvalidOperationException(
                    $"Handler {handler.GetType().Name} does not have a HandleAsync method");
            }

            try
            {
                var task = handleMethod.Invoke(handler, [domainEvent, cancellationToken]) as Task;
                
                if (task != null)
                {
                    await task.ConfigureAwait(false);
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Handler {handler.GetType().Name}.HandleAsync did not return a Task");
                }
            }
            catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
            {
                // Unwrap the target invocation exception to provide clearer error messages
                throw ex.InnerException;
            }
        }
    }

    /// <summary>
    /// Exception thrown when a domain event handler fails to process an event.
    /// </summary>
    public sealed class DomainEventHandlerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEventHandlerException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception that caused the handler to fail.</param>
        public DomainEventHandlerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
