namespace CarService.Domain.Cars;

public interface ICarRepository
{
    public Task<IReadOnlyCollection<Car>> GetCarsAsync(CarSpecification carSpecification, CancellationToken cancellationToken = default);
    public Task<Car?> GetCarByIdAsync(Guid carId, CancellationToken cancellationToken = default);
    public Task AddAsync(Car car, CancellationToken cancellationToken = default);
    public Task UpdateAsync(Car car, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Car car, CancellationToken cancellationToken = default);
}