using System.Reflection;

namespace RentalService.Application.Common;

public static class Permissions
{
    public const string ViewRents = "ViewRents";
    public const string ViewAllRents = "ViewAllRents";
    public const string CreateRent = "CreateRent";
    public const string CreateRentForOthers = "CreateRentForOthers";
    public const string EditRent = "EditRent";
    public const string DeleteRent = "DeleteRent";
    public const string ChangeRentStatus = "ChangeRentStatus";

    public static string[] AllPermissions = typeof(Permissions)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
        .Select(f => (string)f.GetRawConstantValue()!)
        .ToArray();
}
