using System.Reflection;
using UserService.Domain.Common;

namespace UserService.Domain.Permissions;

public class Permissions : Enumeration
{
    // USERS
    public static readonly Permissions ViewUsers = new(1, "ViewUsers");
    public static readonly Permissions EditUsers = new(2, "EditUsers");
    public static readonly Permissions DeleteUsers = new(3, "DeleteUsers");
    public static readonly Permissions CreateUsers = new(4, "CreateUsers");
    public static readonly Permissions ChangeUserStatus = new(5, "ChangeUserStatus");
    public static readonly Permissions ChangeUserRole = new(6, "ChangeUserRole");

    // CONTRACTS
    public static readonly Permissions ViewContracts = new(7, "ViewContracts");
    public static readonly Permissions ViewAllContracts = new(8, "ViewAllContracts"); // 👈 важно

    public static readonly Permissions EditContracts = new(9, "EditContracts");
    public static readonly Permissions DeleteContracts = new(10, "DeleteContracts");

    public static readonly Permissions CreateContracts = new(11, "CreateContracts"); 
    public static readonly Permissions CreateContractsForOthers = new(12, "CreateContractsForOthers"); // 👈 ключевое

    public static readonly Permissions ChangeContractStatus = new(13, "ChangeContractStatus");

    // CARS
    public static readonly Permissions ViewCars = new(14, "ViewCars");
    public static readonly Permissions ViewAllCars = new(15, "ViewAllCars");

    public static readonly Permissions EditCarsDetails = new(16, "EditCarsDetails");
    public static readonly Permissions DeleteCars = new(17, "DeleteCars");
    public static readonly Permissions CreateCars = new(18, "CreateCars");
    public static readonly Permissions ChangeCarsStatus = new(19, "ChangeCarsStatus");

    // RENTS
    public static readonly Permissions ViewRents = new(20, "ViewRents");
    public static readonly Permissions ViewAllRents = new(21, "ViewAllRents");

    public static readonly Permissions CreateRent = new(22, "CreateRent");
    public static readonly Permissions CreateRentForOthers = new(23, "CreateRentForOthers");

    public static readonly Permissions EditRent = new(24, "EditRent");
    public static readonly Permissions DeleteRent = new(25, "DeleteRent");
    public static readonly Permissions ChangeRentStatus = new(26, "ChangeRentStatus");

    public Permissions(int id, string name) : base(id, name) { }

    public static IReadOnlyCollection<Permissions> All =>
        typeof(Permissions)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Permissions))
            .Select(f => (Permissions)f.GetValue(null)!)
            .ToList();
}
