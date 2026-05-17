using Microsoft.EntityFrameworkCore;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Infrastructure.EntitiesConfigurations;

namespace RentalService.Infrastructure;

public class RentalServiceContext(DbContextOptions<RentalServiceContext> options) : DbContext(options)
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
}