using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Common;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Infrastructure.DomainEvents;
using RentalService.Infrastructure.EntitiesConfigurations;

namespace RentalService.Infrastructure;

public class RentalServiceContext(
    IDomainEventDispatcher domainEventDispatcher,
    DbContextOptions<RentalServiceContext> options) : DbContext(options)
{
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new RentalConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentTransactionConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        
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