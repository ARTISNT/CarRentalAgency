using CarService.Application.Exceptions;

namespace CarService.Application.Authorization;

public class CarAuthorizationService(ICarAuthorizationPolicy carAuthorizationPolicy) : ICarAuthorizationService
{
    public void EnsureCanViewCars(Guid? targetClientId = null)
    {
        if (!carAuthorizationPolicy.CanViewCars(targetClientId))
            throw new ForbiddenException("No permission");
    }

    public void EnsureCanViewAllCars()
    {
        if (!carAuthorizationPolicy.CanViewAllCars())
            throw new ForbiddenException("No permission");
    }

    public void EnsureCanCreateCar(Guid? targetClientId = null)
    {
        if (!carAuthorizationPolicy.CanCreateCar(targetClientId))
            throw new ForbiddenException("No permission");
    }

    public void EnsureCanUpdateCar(Guid? targetClientId = null)
    {
        if (!carAuthorizationPolicy.CanUpdateCar(targetClientId))
            throw new ForbiddenException("No permission");
    }

    public void EnsureCanDeleteCar(Guid? targetClientId = null)
    {
        if (!carAuthorizationPolicy.CanDeleteCar(targetClientId))
            throw new ForbiddenException("No permission");
    }

    public void EnsureCanProcessCarReturn()
    {
        if (!carAuthorizationPolicy.CanProcessCarReturn())
            throw new ForbiddenException("No permission");
    }
}
