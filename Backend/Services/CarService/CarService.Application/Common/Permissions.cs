using System.Reflection;

namespace CarService.Application.Common;

public static class Permissions
{
    public const string ViewCars = "ViewCars";
    public const string ViewAllCars = "ViewAllCars";
    public const string ViewCarsForOther = "ViewCarsForOther";

    public const string CreateCars = "CreateCars";
    public const string CreateCarsForOther = "CreateCarsForOther";
    public const string UpdateCars = "EditCarsDetails";
    public const string UpdateCarsForOther = "UpdateCarsForOther";
    public const string DeleteCars = "DeleteCars";
    public const string DeleteCarsForOther = "DeleteCarsForOther";

    public const string ProcessCarReturn = "ProcessCarReturn";

    public static string[] AllPermissions = typeof(Permissions)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
        .Select(f => (string)f.GetRawConstantValue()!)
        .ToArray();
}
