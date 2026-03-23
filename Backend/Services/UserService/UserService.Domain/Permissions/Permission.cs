using UserService.Domain.Common;

namespace UserService.Domain.Permissions;

public class Permission : Enumeration
{
    public static readonly Permission ViewUsers = new Permission(1, "ViewUsers");
    public static readonly Permission EditUsers = new Permission(2, "EditUsers");
    public static readonly Permission DeleteUsers = new Permission(3, "DeleteUsers");
    public static readonly Permission CreateUsers = new Permission(4, "CreateUsers");
    public static readonly Permission ChangeUserStatus = new Permission(5, "ChangeUserStatus");
    public static readonly Permission ChangeUserRole = new Permission(6, "ChangeUserRole");
    
    public static readonly Permission ViewContracts = new Permission(7, "ViewContracts");
    public static readonly Permission EditContracts = new Permission(8, "EditContracts");
    public static readonly Permission DeleteContracts = new Permission(9, "DeleteContracts");
    public static readonly Permission CreateContracts = new Permission(10, "CreateContracts");
    public static readonly Permission ChangeContractStatus = new Permission(11, "ChangeContractStatus");
    
    public static readonly Permission ViewCars = new Permission(12, "ViewCars");
    public static readonly Permission EditCarsDetails = new Permission(13, "EditCarsDetails");
    public static readonly Permission DeleteCars = new Permission(14, "DeleteCarsDetails");
    public static readonly Permission CreateCars = new Permission(15, "CreateCarsDetails");
    public static readonly Permission ChangeCarsStatus = new Permission(16, "ChangeCarsStatus");
    
    public static readonly Permission ViewRents = new Permission(17, "ViewRents");
    public static readonly Permission CreateRent = new Permission(18, "CreateRent");
    public static readonly Permission EditRent = new Permission(19, "EditRent");
    public static readonly Permission DeleteRent = new Permission(20, "DeleteRent");
    public static readonly Permission ChangeRentStatus = new Permission(21, "ChangeRentStatus");
    
    public Permission(int id, string name) : base(id, name) { }
}