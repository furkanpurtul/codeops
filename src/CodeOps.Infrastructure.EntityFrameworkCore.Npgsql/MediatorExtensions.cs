using Microsoft.EntityFrameworkCore;

using CodeOps.Application.Abstractions.Messaging;
using CodeOps.Domain.Abstractions.BuildingBlocks;

namespace CodeOps.Infrastructure.EntityFrameworkCore.Npgsql
{
    internal static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync<T>(this IPublisher publisher, T context, CancellationToken cancellationToken = default)
            where T : DbContext
        {
            ArgumentNullException.ThrowIfNull(publisher);
            ArgumentNullException.ThrowIfNull(context);

            var domainEvents = context.ChangeTracker
                .Entries<IAggregateRoot>()
                .Select(entry => entry.Entity)
                .SelectMany(aggregateRoot => aggregateRoot.DequeueDomainEvents())
                .ToList();

            foreach (var domainEvent in domainEvents)
            {
                await publisher.PublishAsync(domainEvent, cancellationToken);
            }
        }
    }
}
