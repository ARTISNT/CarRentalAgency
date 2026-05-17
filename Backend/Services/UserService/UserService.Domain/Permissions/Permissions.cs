using System.Reflection;
using UserService.Domain.Common;

namespace UserService.Domain.Permissions;

public class Permissions : Enumeration
{
    public static readonly Permissions ViewUsers = new Permissions(1, "ViewUsers");
    public static readonly Permissions EditUsers = new Permissions(2, "EditUsers");
    public static readonly Permissions DeleteUsers = new Permissions(3, "DeleteUsers");
    public static readonly Permissions CreateUsers = new Permissions(4, "CreateUsers");
    public static readonly Permissions ChangeUserStatus = new Permissions(5, "ChangeUserStatus");
    public static readonly Permissions ChangeUserRole = new Permissions(6, "ChangeUserRole");
    
    public static readonly Permissions ViewContracts = new Permissions(7, "ViewContracts");
    public static readonly Permissions EditContracts = new Permissions(8, "EditContracts");
    public static readonly Permissions DeleteContracts = new Permissions(9, "DeleteContracts");
    public static readonly Permissions CreateContracts = new Permissions(10, "CreateContracts");
    public static readonly Permissions ChangeContractStatus = new Permissions(11, "ChangeContractStatus");
    
    public static readonly Permissions ViewCars = new Permissions(12, "ViewCars");
    public static readonly Permissions EditCarsDetails = new Permissions(13, "EditCarsDetails");
    public static readonly Permissions DeleteCars = new Permissions(14, "DeleteCarsDetails");
    public static readonly Permissions CreateCars = new Permissions(15, "CreateCarsDetails");
    public static readonly Permissions ChangeCarsStatus = new Permissions(16, "ChangeCarsStatus");
    
    public static readonly Permissions ViewRents = new Permissions(17, "ViewRents");
    public static readonly Permissions CreateRent = new Permissions(18, "CreateRent");
    public static readonly Permissions EditRent = new Permissions(19, "EditRent");
    public static readonly Permissions DeleteRent = new Permissions(20, "DeleteRent");
    public static readonly Permissions ChangeRentStatus = new Permissions(21, "ChangeRentStatus");
    
    public Permissions(int id, string name) : base(id, name) { }

    public static IReadOnlyCollection<Permissions> All => 
    typeof(Permissions)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(Permissions))
            .Select(f => (Permissions)f.GetValue(null)!)
            .ToList();
}