using UserService.Domain.Common;
using UserService.Domain.Permissions;

namespace UserService.Domain.Roles;

public class Role : Enumeration
{
    private static IEnumerable<Permissions.Permissions> BaseManagerPermissions()
    {
        return
        [
            Domain.Permissions.Permissions.ViewUsers,
            Domain.Permissions.Permissions.ViewCars,
            Domain.Permissions.Permissions.EditCarsDetails,
            Domain.Permissions.Permissions.DeleteCars,
            Domain.Permissions.Permissions.CreateCars,
            Domain.Permissions.Permissions.ChangeCarsStatus,
            Domain.Permissions.Permissions.ViewRents,
            Domain.Permissions.Permissions.CreateRent,
            Domain.Permissions.Permissions.EditRent,
            Domain.Permissions.Permissions.DeleteRent,
            Domain.Permissions.Permissions.ChangeRentStatus,
            Domain.Permissions.Permissions.ViewContracts,
            Domain.Permissions.Permissions.EditContracts,
            Domain.Permissions.Permissions.DeleteContracts,
            Domain.Permissions.Permissions.CreateContracts,
            Domain.Permissions.Permissions.ChangeContractStatus
        ];
    }

    public static readonly Role Manager =
        new Role(2, "Manager", BaseManagerPermissions());

    public static readonly Role Admin =
        new Role(1, "Admin",
            BaseManagerPermissions().Concat(new[]
            {
                Domain.Permissions.Permissions.EditUsers,
                Domain.Permissions.Permissions.DeleteUsers,
                Domain.Permissions.Permissions.CreateUsers,
                Domain.Permissions.Permissions.ChangeUserRole,
                Domain.Permissions.Permissions.ChangeUserStatus
            }));

    public static readonly Role Client =
        new Role(3, "Client",
            new[]
            {
                Domain.Permissions.Permissions.ViewCars,
                Domain.Permissions.Permissions.CreateRent
            });

    private readonly HashSet<Permissions.Permissions> _permissions;

    public IReadOnlyCollection<Permissions.Permissions> Permissions => _permissions;

    private Role(int id, string name, IEnumerable<Permissions.Permissions> permissions)
        : base(id, name)
    {
        _permissions = new HashSet<Permissions.Permissions>(permissions);
    }

    public bool HasPermission(Permissions.Permissions permissionS)
        => _permissions.Contains(permissionS);
    
    public override string ToString() => Name;
}
