namespace CarService.Application.Authorization;

public interface ICarAuthorizationService
{
    void EnsureCanViewCars(Guid? targetClientId = null);
    void EnsureCanViewAllCars();
    void EnsureCanCreateCar(Guid? targetClientId = null);
    void EnsureCanUpdateCar(Guid? targetClientId = null);
    void EnsureCanDeleteCar(Guid? targetClientId = null);
    void EnsureCanProcessCarReturn();
}
