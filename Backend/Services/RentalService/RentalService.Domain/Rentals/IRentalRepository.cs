namespace RentalService.Domain.Rentals;

public interface IRentalRepository
{
    public Task<IReadOnlyCollection<Rental>> GetRentalsAsync(CancellationToken cancellationToken = default);
    public Task<Rental?> GetRentalAsync(Guid rentalId, CancellationToken cancellationToken = default);
    public Task AddRentalAsync(Rental rental,  CancellationToken cancellationToken = default);
    public Task UpdateRentalAsync(Rental rental, CancellationToken cancellationToken = default);
}