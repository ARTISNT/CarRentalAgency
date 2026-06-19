using ContractService.Domain.Contracts;

namespace ContractService.Application.Services;

public class ContractTemplateVariablesBuilder
{
    public IReadOnlyDictionary<string, object?> ForContract(Contract contract)
    {
        var dict = BuildBase(contract);
        return dict;
    }

    public IReadOnlyDictionary<string, object?> ForAddition(Contract contract, ContractAddition addition)
    {
        var dict = BuildBase(contract);
        dict["addition"] = new Dictionary<string, object?>
        {
            ["PreviousEndDate"] = addition.PreviousEndDate,
            ["NewEndDate"] = addition.NewEndDate,
            ["AdditionalCost"] = addition.AdditionalCost,
            ["CreatedAt"] = addition.CreatedAt,
        };
        return dict;
    }

    public IReadOnlyDictionary<string, object?> ForReturnAct(Contract contract, ContractReturnAct returnAct)
    {
        var dict = BuildBase(contract);
        dict["returnAct"] = new Dictionary<string, object?>
        {
            ["Mileage"] = returnAct.Mileage,
            ["FuelLevel"] = returnAct.FuelLevel,
            ["PenaltyAmount"] = returnAct.PenaltyAmount,
            ["DamageDescription"] = returnAct.DamageDescription ?? string.Empty,
            ["CreatedAt"] = returnAct.CreatedAt,
        };
        return dict;
    }

    private static Dictionary<string, object?> BuildBase(Contract contract)
    {
        return new Dictionary<string, object?>
        {
            ["contract"] = new Dictionary<string, object?>
            {
                ["Id"] = contract.Id.ToString(),
                ["CreatedAt"] = contract.CreatedAt,
                ["ActualEndDate"] = contract.ActualEndDate,
            },
            ["client"] = new Dictionary<string, object?>
            {
                ["Surname"] = contract.Client.Surname,
                ["Name"] = contract.Client.Name,
                ["Patronymic"] = contract.Client.Patronymic,
                ["FullName"] = string.Join(" ",
                    contract.Client.Surname,
                    contract.Client.Name,
                    contract.Client.Patronymic).Trim(),
                ["PhoneNumber"] = contract.Client.PhoneNumber,
                ["PassportNumber"] = contract.Client.PassportNumber,
                ["PassportIdentificationNumber"] = contract.Client.PassportIdentificationNumber,
                ["BirthDate"] = contract.Client.BirthDate,
                ["PassportIssueDate"] = contract.Client.PassportIssueDate,
            },
            ["car"] = new Dictionary<string, object?>
            {
                ["Brand"] = contract.Car.Brand,
                ["Model"] = contract.Car.Model,
                ["FullName"] = $"{contract.Car.Brand} {contract.Car.Model}".Trim(),
                ["LicensePlate"] = contract.Car.LicensePlate,
                ["Color"] = contract.Car.Color,
                ["CarBodyStyle"] = contract.Car.CarBodyStyle,
            },
            ["rental"] = new Dictionary<string, object?>
            {
                ["StartDate"] = contract.Rental.StartDate,
                ["EndDate"] = contract.Rental.EndDate,
                ["EstimatedPrice"] = contract.Rental.EstimatedPrice,
            },
            ["currentDate"] = DateTime.UtcNow,
            ["currentYear"] = DateTime.UtcNow.Year,
        };
    }
}
