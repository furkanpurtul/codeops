using System.Reflection;

using CodeOps.Application.Abstractions.Messaging;
using CodeOps.Application.Abstractions.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CodeOps.Infrastructure.EntityFrameworkCore.Npgsql
{
    /// <summary>
    /// Add-Migration InitialCreate -Args '--environment dev'
    /// Update-Database -Args '--environment dev'
    /// dotnet ef migrations add InitialCreate --project src/CodeOps.Infrastructure.EntityFrameworkCore.Npgsql --startup-project src/CodeOps.Api -- --environment dev
    /// dotnet ef database update --project src/CodeOps.Infrastructure.EntityFrameworkCore.Npgsql --startup-project src/CodeOps.Api -- --environment dev
    /// </summary>
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IPublisher? _publisher;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher? publisher = null) : base(options)
        {
            _publisher = publisher;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        { 
            if (_publisher is not null)
                await _publisher.DispatchDomainEventsAsync(this);
            
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
#endif

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
