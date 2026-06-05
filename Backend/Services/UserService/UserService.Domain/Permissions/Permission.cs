using UserService.Domain.Common;

namespace UserService.Domain.Permissions;

public class Permission : Enumeration
{
    public static readonly Permission ViewUsers = new(1, "ViewUsers");
    public static readonly Permission EditUsers = new(2, "EditUsers");
    public static readonly Permission DeleteUsers = new(3, "DeleteUsers");
    public static readonly Permission CreateUsers = new(4, "CreateUsers");
    public static readonly Permission ChangeUserStatus = new(5, "ChangeUserStatus");
    public static readonly Permission ChangeUserRole = new(6, "ChangeUserRole");

    public static readonly Permission ViewContracts = new(7, "ViewContracts");
    public static readonly Permission ViewAllContracts = new(8, "ViewAllContracts"); 

    public static readonly Permission CancelContracts = new(9, "CancelContracts");
    public static readonly Permission SignContracts = new(10, "SignContracts");

    public static readonly Permission CreateContracts = new(11, "CreateContracts"); 
    public static readonly Permission CreateContractsForOthers = new(12, "CreateContractsForOthers"); 

    public static readonly Permission ChangeContractStatus = new(13, "ChangeContractStatus");

    public static readonly Permission ViewCars = new(14, "ViewCars");
    public static readonly Permission ViewAllCars = new(15, "ViewAllCars");

    public static readonly Permission EditCarsDetails = new(16, "EditCarsDetails");
    public static readonly Permission DeleteCars = new(17, "DeleteCars");
    public static readonly Permission CreateCars = new(18, "CreateCars");
    public static readonly Permission ChangeCarsStatus = new(19, "ChangeCarsStatus");

    public static readonly Permission ViewRents = new(20, "ViewRents");
    public static readonly Permission ViewAllRents = new(21, "ViewAllRents");

    public static readonly Permission CreateRent = new(22, "CreateRent");
    public static readonly Permission CreateRentForOthers = new(23, "CreateRentForOthers");

    public static readonly Permission EditRent = new(24, "EditRent");
    public static readonly Permission DeleteRent = new(25, "DeleteRent");
    public static readonly Permission ChangeRentStatus = new(26, "ChangeRentStatus");

    public Permission(int id, string name) : base(id, name) { }

    public static IReadOnlyCollection<Permission> All =>
        typeof(Permission)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Permission))
            .Select(f => (Permission)f.GetValue(null)!)
            .ToList();
}
