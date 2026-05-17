using RentalService.Application.Features.Rentals.CreateRental;

namespace RentalService.Application.Common;

public interface ICarExternalService
{
    public Task<CarForRentResponse> GetCarForRentAsync(Guid userId);
}