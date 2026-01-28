using Microsoft.Extensions.DependencyInjection;
using CodeOps.Domain.Abstractions.Samples.Entities;
using CodeOps.Domain.Abstractions.Samples.Events;
using CodeOps.Domain.Abstractions.Samples.EventHandlers;
using CodeOps.Domain.Abstractions.Samples.ValueObjects;

namespace CodeOps.Domain.Abstractions.Samples
{
    /// <summary>
    /// Sample application demonstrating how to configure and use the domain event dispatcher.
    /// </summary>
    /// <remarks>
    /// This example shows:
    /// 1. How to register the dispatcher and handlers with dependency injection
    /// 2. How to create aggregates that raise domain events
    /// 3. How to dispatch events after persisting changes
    /// </remarks>
    public static class DomainEventDispatcherSample
    {
        /// <summary>
        /// Configures services for the domain event dispatcher sample.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <returns>The configured service collection.</returns>
        public static IServiceCollection AddDomainEventDispatcherSample(this IServiceCollection services)
        {
            // Register the domain event dispatcher
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();

            // Register domain event handlers
            // Note: Use AddTransient for handlers that have dependencies on scoped services (e.g., DbContext)
            services.AddTransient<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
            services.AddTransient<IDomainEventHandler<OrderCancelledEvent>, OrderCancelledEventHandler>();

            return services;
        }

        /// <summary>
        /// Runs a sample scenario demonstrating the domain event dispatcher.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task RunSampleAsync()
        {
            // Setup dependency injection
            var services = new ServiceCollection();
            services.AddDomainEventDispatcherSample();
            var serviceProvider = services.BuildServiceProvider();

            // Get the dispatcher
            var dispatcher = serviceProvider.GetRequiredService<IDomainEventDispatcher>();

            Console.WriteLine("=== Domain Event Dispatcher Sample ===\n");

            // Example 1: Create an order and dispatch events
            Console.WriteLine("Example 1: Creating an order");
            var orderId = OrderId.New();
            var lines = new[]
            {
                OrderLine.Create(
                    OrderLineId.New(),
                    "SKU-001",
                    1,
                    Money.Of(100.00m, "USD"))
            };

            var order = Order.Create(orderId, lines);
            Console.WriteLine($"Order {orderId.Value} created with {order.DomainEvents.Count} event(s)");

            // Dispatch events from the aggregate
            await dispatcher.DispatchAsync(order);
            Console.WriteLine();

            // Example 2: Cancel an order and dispatch events
            Console.WriteLine("Example 2: Cancelling an order");
            order.Cancel("Customer requested cancellation");
            Console.WriteLine($"Order has {order.DomainEvents.Count} event(s) after cancellation");

            // Dispatch events individually
            var events = order.DequeueDomainEvents();
            foreach (var evt in events)
            {
                await dispatcher.DispatchAsync(evt);
            }
            Console.WriteLine();

            // Example 3: Creating and dispatching multiple orders
            Console.WriteLine("Example 3: Processing multiple orders");
            var order2 = Order.Create(OrderId.New(), lines);
            var order3 = Order.Create(OrderId.New(), lines);

            var allEvents = order2.DequeueDomainEvents()
                .Concat(order3.DequeueDomainEvents())
                .ToList();

            await dispatcher.DispatchAsync(allEvents);
            Console.WriteLine();

            Console.WriteLine("=== Sample Completed ===");
        }
    }
}
