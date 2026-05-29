namespace ContractService.Requests;

public record CreateContractRequest(Guid ClientId, Guid RentalId, Guid CarId, Guid ContractTemplateId);