using RentalService.Application.Features.Rentals.CreateRental;

namespace RentalService.Application.Common;

public interface IUserExternalService
{
    public Task<UserRentInfoResponse> GetUserForRentAsync(Guid userId);
}