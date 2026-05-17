using CarService.Domain.Cars;
using CarService.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure;

public class CarServiceDbContext(DbContextOptions<CarServiceDbContext> dbContextOptions) : 
    DbContext(dbContextOptions)
{
    public DbSet<Car> Cars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CarConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}