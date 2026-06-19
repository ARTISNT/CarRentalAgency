namespace RentalService.Application.Authorization;

public interface IRentalAuthorizationService
{
    void EnsureCanViewRentals(Guid ownerId);
    void EnsureCanViewAllRentals();
    void EnsureCanCreateRental(Guid targetClientId);
    void EnsureCanEditRental();
    void EnsureCanDeleteRental();
    void EnsureCanChangeRentStatus();
    void EnsureCanRequestReturn(Guid ownerId);
}
