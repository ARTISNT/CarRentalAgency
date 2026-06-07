using System.Reflection;

namespace ContractService.Application.Common;

public static class Permissions
{
    public const string ViewContracts = "ViewContracts";
    public const string ViewAllContracts = "ViewAllContracts"; 
    
    public const string CreateContracts = "CreateContracts";
    public const string CreateContractsForOthers = "CreateContractsForOthers";
    
    public const string CancelContracts = "CancelContracts";
    public const string SignContracts = "SignContracts";
    public const string ChangeContractStatus = "ChangeContractStatus";
    
    public static string[] AllPermissions = typeof(Permissions)
        .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
        .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
        .Select(f => (string)f.GetRawConstantValue())
        .ToArray(); 
}