using ContractService.Domain.Common;

namespace ContractService.Domain.Contracts;

public sealed record ContractAddition : IValueObject
{
    public DateTime PreviousEndDate { get; init; }
    public DateTime NewEndDate { get; init; }
    public decimal AdditionalCost { get; init; }
    public DateTime CreatedAt { get; init; }
    public ContractTemplateSnapshot Template { get; init; }

    private ContractAddition() { }

    public ContractAddition(
        DateTime previousEndDate,
        DateTime newEndDate,
        decimal additionalCost,
        ContractTemplateSnapshot template)
    {
        ValidateInputs(previousEndDate, newEndDate, additionalCost);
        ValidateTemplate(template);

        PreviousEndDate = previousEndDate;
        NewEndDate = newEndDate;
        AdditionalCost = additionalCost;
        Template = template;
        CreatedAt = DateTime.UtcNow;
    }

    private static void ValidateInputs(
        DateTime previousEndDate, 
        DateTime newEndDate, 
        decimal additionalCost)
    {
        if (previousEndDate == default)
            throw new ArgumentException(
                "Previous end date is required", 
                nameof(previousEndDate));

        if (newEndDate == default)
            throw new ArgumentException(
                "New end date is required", 
                nameof(newEndDate));

        if (newEndDate <= previousEndDate)
            throw new InvalidOperationException(
                "New end date must be greater than previous end date");

        if (additionalCost < 0)
            throw new ArgumentOutOfRangeException(
                nameof(additionalCost), 
                "Additional cost cannot be negative");
    }

    private static void ValidateTemplate(ContractTemplateSnapshot template)
    {
        ArgumentNullException.ThrowIfNull(template);

        if (template.DocumentType != DocumentType.Addition.Name)
            throw new InvalidOperationException(
                "Template document type must be Addition");

        if (!template.IsActive)
            throw new InvalidOperationException(
                "Template must be active");
    }
}
