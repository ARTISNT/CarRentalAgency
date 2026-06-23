using RentalService.Application.Abstractions.Security;
using RentalService.Application.Common;

namespace RentalService.Application.Authorization;

public class RentalAuthorizationPolicy(IClientContext clientContext) : IRentalAuthorizationPolicy
{
    public bool CanViewRental(Guid ownerId)
    {
        var isOwnRental = ownerId == clientContext.ClientId;
        if (isOwnRental)
            return HasPermission(Permissions.ViewRents);
        return HasPermission(Permissions.ViewAllRents);
    }

    public bool CanViewAllRentals() => HasPermission(Permissions.ViewAllRents);

    public bool CanCreateRental(Guid targetClientId)
    {
        var isOtherClient = targetClientId != clientContext.ClientId;
        if (isOtherClient)
            return HasPermission(Permissions.CreateRentForOthers);
        return HasPermission(Permissions.CreateRent);
    }

    public bool CanEditRental() => HasPermission(Permissions.EditRent);

    public bool CanDeleteRental() => HasPermission(Permissions.DeleteRent);

    public bool CanChangeRentStatus() => HasPermission(Permissions.ChangeRentStatus);

    public bool CanRequestReturn(Guid ownerId)
    {
        if (ownerId == clientContext.ClientId)
            return HasPermission(Permissions.ViewRents);

        // staff (Manager/Admin с EditRent) может подавать заявку на возврат за любого клиента,
        // в т.ч. за самого себя, если у аренды он выступает как renter.
        return HasPermission(Permissions.EditRent);
    }

    private bool HasPermission(string permission) => clientContext.Permissions.Contains(permission);
}
