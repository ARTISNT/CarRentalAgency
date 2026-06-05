using ContractService.Domain.Common;

namespace ContractService.Domain.Contracts;

public record ContractReturnAct : IValueObject
{
    public DateTime CreatedAt { get; init; }
    public int Mileage { get; init; }
    public decimal FuelLevel { get; init; }
    public decimal PenaltyAmount { get; init; }
    public string? DamageDescription { get; init; }
    public ContractTemplateSnapshot Template { get; init; }

    private const int MaxDamageDescriptionLength = 1000;

    private ContractReturnAct() { }

    public ContractReturnAct(
        int mileage,
        decimal fuelLevel,
        decimal penaltyAmount,
        string? damageDescription,
        ContractTemplateSnapshot template)
    {
        ValidateMetrics(mileage, fuelLevel, penaltyAmount);
        ValidateDamageDescription(damageDescription);
        ValidateTemplate(template);

        Mileage = mileage;
        FuelLevel = fuelLevel;
        PenaltyAmount = penaltyAmount;
        DamageDescription = damageDescription;
        Template = template;
        
        CreatedAt = DateTime.UtcNow;
    }
    
    private static void ValidateMetrics(int mileage, decimal fuelLevel, decimal penaltyAmount)
    {
        if (mileage < 0)
            throw new ArgumentOutOfRangeException(
                nameof(mileage),
                "Mileage cannot be negative");

        if (fuelLevel < 0 || fuelLevel > 100)
            throw new ArgumentOutOfRangeException(
                nameof(fuelLevel),
                "Fuel level must be between 0 and 100");

        if (penaltyAmount < 0)
            throw new ArgumentOutOfRangeException(
                nameof(penaltyAmount),
                "Penalty amount cannot be negative");
    }

    private static void ValidateDamageDescription(string? damageDescription)
    {
        if (!string.IsNullOrWhiteSpace(damageDescription) &&
            damageDescription.Length > MaxDamageDescriptionLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(damageDescription),
                $"Damage description length cannot exceed {MaxDamageDescriptionLength}");
        }
    }

    private static void ValidateTemplate(ContractTemplateSnapshot template)
    {
        _ = template ?? throw new ArgumentNullException(nameof(template));

        if (template.DocumentType != DocumentType.ReturnAct.Name)
            throw new InvalidOperationException(
                "Template document type must be ReturnAct");

        if (!template.IsActive)
            throw new InvalidOperationException(
                "Template must be active");
    }
}
