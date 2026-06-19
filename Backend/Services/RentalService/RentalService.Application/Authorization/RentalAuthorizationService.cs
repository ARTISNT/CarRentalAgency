using RentalService.Application.Exceptions;

namespace RentalService.Application.Authorization;

public class RentalAuthorizationService(IRentalAuthorizationPolicy rentalAuthorizationPolicy) : IRentalAuthorizationService
{
    public void EnsureCanViewRentals(Guid ownerId)
    {
        if (!rentalAuthorizationPolicy.CanViewRental(ownerId))
            throw new ForbiddenException("You do not have permission to view this rental");
    }

    public void EnsureCanViewAllRentals()
    {
        if (!rentalAuthorizationPolicy.CanViewAllRentals())
            throw new ForbiddenException("You do not have permission to view all rentals");
    }

    public void EnsureCanCreateRental(Guid targetClientId)
    {
        if (!rentalAuthorizationPolicy.CanCreateRental(targetClientId))
            throw new ForbiddenException("You do not have permission to create this rental");
    }

    public void EnsureCanEditRental()
    {
        if (!rentalAuthorizationPolicy.CanEditRental())
            throw new ForbiddenException("You do not have permission to edit this rental");
    }

    public void EnsureCanDeleteRental()
    {
        if (!rentalAuthorizationPolicy.CanDeleteRental())
            throw new ForbiddenException("You do not have permission to delete this rental");
    }

    public void EnsureCanChangeRentStatus()
    {
        if (!rentalAuthorizationPolicy.CanChangeRentStatus())
            throw new ForbiddenException("You do not have permission to change rental status");
    }

    public void EnsureCanRequestReturn(Guid ownerId)
    {
        if (!rentalAuthorizationPolicy.CanRequestReturn(ownerId))
            throw new ForbiddenException("You do not have permission to request return for this rental");
    }
}
