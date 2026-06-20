using Microsoft.EntityFrameworkCore;
using UserService.Application.EmailOutbox;
using UserService.Domain.Common;
using UserService.Domain.Users;
using UserService.Infrastructure.DomainEvents;
using UserService.Infrastructure.EmailOutbox;
using UserService.Infrastructure.EntityConfiguration;

namespace UserService.Infrastructure;

public class UserServiceContext(IDomainEventDispatcher domainEventDispatcher,
    DbContextOptions<UserServiceContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<EmailOutboxEntry> EmailOutbox { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new EmailOutboxConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        int result = await base.SaveChangesAsync(cancellationToken);
        await PublishDomainEventsAsync(cancellationToken);
        
        return result;
    }

    private async Task PublishDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Select(x => x.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();
                return domainEvents;
            }).ToList();

        await domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
    } 
}