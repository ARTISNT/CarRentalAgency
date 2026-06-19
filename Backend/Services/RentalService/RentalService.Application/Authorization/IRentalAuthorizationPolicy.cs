namespace RentalService.Application.Authorization;

public interface IRentalAuthorizationPolicy
{
    bool CanViewRental(Guid ownerId);
    bool CanViewAllRentals();
    bool CanCreateRental(Guid targetClientId);
    bool CanEditRental();
    bool CanDeleteRental();
    bool CanChangeRentStatus();
    bool CanRequestReturn(Guid ownerId);
}
