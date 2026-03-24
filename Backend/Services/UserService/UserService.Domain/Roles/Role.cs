using UserService.Domain.Common;
using UserService.Domain.Permissions;

namespace UserService.Domain.Roles;

public class Role : Enumeration
{
    private static IEnumerable<Permission> BaseManagerPermissions()
    {
        return
        [
            Permission.ViewUsers,
            Permission.ViewCars,
            Permission.EditCarsDetails,
            Permission.DeleteCars,
            Permission.CreateCars,
            Permission.ChangeCarsStatus,
            Permission.ViewRents,
            Permission.CreateRent,
            Permission.EditRent,
            Permission.DeleteRent,
            Permission.ChangeRentStatus,
            Permission.ViewContracts,
            Permission.EditContracts,
            Permission.DeleteContracts,
            Permission.CreateContracts,
            Permission.ChangeContractStatus
        ];
    }

    public static readonly Role Manager =
        new Role(2, "Manager", BaseManagerPermissions());

    public static readonly Role Admin =
        new Role(1, "Admin",
            BaseManagerPermissions().Concat(new[]
            {
                Permission.EditUsers,
                Permission.DeleteUsers,
                Permission.CreateUsers,
                Permission.ChangeUserRole,
                Permission.ChangeUserStatus
            }));

    public static readonly Role Client =
        new Role(3, "Client",
            new[]
            {
                Permission.ViewCars,
                Permission.CreateRent
            });

    private readonly HashSet<Permission> _permissions;

    public IReadOnlyCollection<Permission> Permissions => _permissions;

    private Role(int id, string name, IEnumerable<Permission> permissions)
        : base(id, name)
    {
        _permissions = new HashSet<Permission>(permissions);
    }

    public bool HasPermission(Permission permission)
        => _permissions.Contains(permission);
    
    public override string ToString() => Name;
}
