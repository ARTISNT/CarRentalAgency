using CarService.Application.Abstractions.Security;
using CarService.Application.Common;

namespace CarService.Application.Authorization;

public class CarAuthorizationPolicy(IClientContext clientContext) : ICarAuthorizationPolicy
{
    public bool CanViewCars(Guid? targetClientId = null)
    {
        if (targetClientId.HasValue && targetClientId.Value != clientContext.ClientId)
            return HasPermission(Permissions.ViewCarsForOther);

        return HasPermission(Permissions.ViewCars);
    }

    public bool CanViewAllCars()
    {
        return HasPermission(Permissions.ViewAllCars);
    }

    public bool CanCreateCar(Guid? targetClientId = null)
    {
        if (targetClientId.HasValue && targetClientId.Value != clientContext.ClientId)
            return HasPermission(Permissions.CreateCarsForOther);

        return HasPermission(Permissions.CreateCars);
    }

    public bool CanUpdateCar(Guid? targetClientId = null)
    {
        if (targetClientId.HasValue && targetClientId.Value != clientContext.ClientId)
            return HasPermission(Permissions.UpdateCarsForOther);

        return HasPermission(Permissions.UpdateCars);
    }

    public bool CanDeleteCar(Guid? targetClientId = null)
    {
        if (targetClientId.HasValue && targetClientId.Value != clientContext.ClientId)
            return HasPermission(Permissions.DeleteCarsForOther);

        return HasPermission(Permissions.DeleteCars);
    }

    public bool CanProcessCarReturn()
    {
        return HasPermission(Permissions.ProcessCarReturn);
    }

    private bool HasPermission(string permission)
    {
        return clientContext.Permissions.Contains(permission);
    }
}
