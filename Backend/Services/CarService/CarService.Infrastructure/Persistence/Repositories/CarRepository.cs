using CarService.Domain.Cars;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure.Persistence.Repositories;

public class CarRepository(CarServiceDbContext dbContext) : ICarRepository
{
    public async Task<IReadOnlyCollection<Car>> GetCarsAsync(CancellationToken cancellationToken = default)
    {
        var cars = await dbContext.Cars.ToListAsync(cancellationToken);
        return cars;
    }

    public async Task<Car?> GetCarByIdAsync(Guid carId, CancellationToken cancellationToken = default)
    {
        var car = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == carId, cancellationToken);
        return car;
    }

    public async Task AddAsync(Car car, CancellationToken cancellationToken = default)
    {
        await dbContext.Cars.AddAsync(car, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Car car, CancellationToken cancellationToken = default)
    {
        dbContext.Cars.Update(car);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Car car, CancellationToken cancellationToken = default)
    {
        dbContext.Cars.Remove(car);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}