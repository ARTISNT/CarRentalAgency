using UserService.Domain.Common;
using Permission = UserService.Domain.Permissions.Permission;

namespace UserService.Domain.Roles;

public class Role : Enumeration
{
    private readonly HashSet<Permissions.Permission> _permissions;

    public IReadOnlyCollection<Permission> Permissions => _permissions;

    private Role(int id, string name, IEnumerable<Permission> permissions)
        : base(id, name)
    {
        _permissions = new HashSet<Permission>(permissions);
    }

    public bool HasPermission(Permission permission)
        => _permissions.Contains(permission);

    private static IEnumerable<Permissions.Permission> BaseManagerPermissions()
    {
        return
        [
            Permission.ViewUsers,
            Permission.ProcessCarReturn,

            Permission.InteractWithContractTemplates,
            Permission.ViewCars,
            Permission.ViewAllCars,
            Permission.EditCarsDetails,
            Permission.DeleteCars,
            Permission.CreateCars,
            Permission.ChangeCarsStatus,

            Permission.ViewRents,
            Permission.ViewAllRents,
            Permission.CreateRent,
            Permission.CreateRentForOthers,
            Permission.EditRent,
            Permission.DeleteRent,
            Permission.ChangeRentStatus,

            Permission.ViewContracts,
            Permission.ViewAllContracts,
            Permission.CancelContracts,
            Permission.SignContracts,
            Permission.CreateContracts,
            Permission.CreateContractsForOthers,
            Permission.ChangeContractStatus
        ];
    }

    public static readonly Role Manager =
        new Role(2, "Manager", BaseManagerPermissions());

    public static readonly Role Admin =
        new Role(1, "Admin", BaseManagerPermissions().Concat(new[]
        {
            Permission.EditUsers,
            Permission.DeleteUsers,
            Permission.CreateUsers,
            Permission.ChangeUserRole,
            Permission.ChangeUserStatus,
            Permission.ViewAllContracts,
            Permission.ViewAllRents,
            Permission.ViewAllCars
        }));

    public static readonly Role Client =
        new Role(3, "Client", new[]
        {
            Permission.ViewCars,
            Permission.ViewContracts,
            Permission.ViewRents,
            Permission.CreateRent,
            Permission.CreateContracts,
            Permission.SignContracts,
        });

    public override string ToString() => Name;
}
