namespace CarService.Application.Authorization;

public interface ICarAuthorizationPolicy
{
    bool CanViewCars(Guid? targetClientId = null);
    bool CanViewAllCars();
    bool CanCreateCar(Guid? targetClientId = null);
    bool CanUpdateCar(Guid? targetClientId = null);
    bool CanDeleteCar(Guid? targetClientId = null);
    bool CanProcessCarReturn();
}
