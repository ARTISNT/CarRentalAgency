using AutoMapper;
using Contracts.ContractEvents;
using ContractService.Application.Abstractions.Services;
using ContractService.Application.Exceptions.Contracts;
using ContractService.Application.Options;
using ContractService.Application.Services;
using ContractService.Domain.Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;

namespace ContractService.Application.Features.Contracts.EndContract;

public class EndContractByRentalCommandHandler(
    IContractRepository contractRepository,
    IContractTemplateRepository contractTemplateRepository,
    IOptions<DocumentTemplateOptions> options,
    IMapper mapper,
    ContractDocumentService documentService,
    IPublishEndpoint publishEndpoint,
    ITemplateRenderer templateRenderer,
    ContractTemplateVariablesBuilder variablesBuilder)
    : IRequestHandler<EndContractByRentalCommand>
{
    public async Task Handle(EndContractByRentalCommand request, CancellationToken cancellationToken)
    {
        var contract = await contractRepository.GetContractByRentalIdAsync(request.RentalId, cancellationToken)
            ?? throw new ContractNotFoundException("Contract not found for rental");

        var template = await contractTemplateRepository.GetContractTemplatesAsync(
                options.Value.ReturnActTemplateId, cancellationToken)
            ?? throw new ContractNotFoundException("ReturnAct template not found");

        var templateSnapshot = mapper.Map<ContractTemplateSnapshot>(template);

        var returnAct = new ContractReturnAct(
            request.Mileage,
            request.FuelLevel,
            request.PenaltyAmount,
            request.DamageDescription,
            templateSnapshot);

        contract.EndContract(returnAct);

        await contractRepository.UpdateContractAsync(contract, cancellationToken);

        var variables = variablesBuilder.ForReturnAct(contract, returnAct);
        var renderedContent = templateRenderer.Render(template.Content, variables);

        await documentService.GenerateReturnAct(contract.ClientId, renderedContent, contract);
        documentService.SignReturnAct(contract.ClientId, contract);

        // === Досрочный возврат: доп. соглашение о расторжении ===
        if (request.ReturnDate < contract.Rental.EndDate)
        {
            var additionTemplate = await contractTemplateRepository.GetContractTemplatesAsync(
                    options.Value.AdditionTemplateId, cancellationToken)
                ?? throw new ContractNotFoundException("Addition template not found");

            var additionTemplateSnapshot = mapper.Map<ContractTemplateSnapshot>(additionTemplate);

            // ContractAddition требует NewEndDate > PreviousEndDate.
            // Для терминации меняем местами: "предыдущая" = фактический возврат,
            // "новая" = плановый срок, чтобы валидация прошла.
            // Шаблон использует termination.*, поэтому семантика для UI корректна.
            var termination = new ContractAddition(
                request.ReturnDate,
                contract.Rental.EndDate,
                0m,
                additionTemplateSnapshot);
            contract.AddTerminationAddition(termination);

            await contractRepository.UpdateContractAsync(contract, cancellationToken);

            var daysSaved = (int)Math.Ceiling((contract.Rental.EndDate - request.ReturnDate).TotalDays);

            var terminationVariables = new Dictionary<string, object?>
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
                ["addition"] = new Dictionary<string, object?>
                {
                    ["PreviousEndDate"] = contract.Rental.EndDate,
                    ["NewEndDate"] = request.ReturnDate,
                    ["AdditionalCost"] = 0m,
                    ["CreatedAt"] = DateTime.UtcNow,
                },
                ["termination"] = new Dictionary<string, object?>
                {
                    ["PreviousEndDate"] = contract.Rental.EndDate,
                    ["ActualReturnDate"] = request.ReturnDate,
                    ["DaysSaved"] = daysSaved,
                },
                ["currentDate"] = DateTime.UtcNow,
                ["currentYear"] = DateTime.UtcNow.Year,
            };

            var renderedTermination = templateRenderer.Render(
                additionTemplateSnapshot.Content,
                terminationVariables);

            await documentService.GenerateAddition(
                contract.ClientId,
                renderedTermination,
                contract);
            documentService.SignAddition(contract.ClientId, contract);
        }

        await publishEndpoint.Publish(new ContractEndedIntegrationEvent(
            contract.Id,
            contract.RentalId,
            contract.ContractAutoId,
            contract.ClientId,
            request.Mileage,
            request.FuelLevel,
            request.PenaltyAmount,
            request.DamageDescription,
            DateTime.UtcNow), cancellationToken);
    }
}
