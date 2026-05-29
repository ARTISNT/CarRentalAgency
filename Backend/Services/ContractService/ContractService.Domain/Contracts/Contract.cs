using ContractService.Domain.Common;
using ContractService.Domain.DomainEvents;

namespace ContractService.Domain.Contracts;

public sealed class Contract : Entity, IAggregateRoot
{
    public Guid ContractTemplateId { get; private set; }
    public ContractStatus Status { get; private set; }
    public ClientSnapshot Client { get; }
    public ContractAutoSnapshot Car { get; }
    public ContractTemplateSnapshot Template { get; }
    public RentalSnapshot Rental { get; }

    private Contract() {}
    
    public Contract(Guid contractTemplateId,
        ClientSnapshot client,
        ContractAutoSnapshot car,
        ContractTemplateSnapshot template,
        RentalSnapshot rental)
    {
        if(contractTemplateId  == Guid.Empty)
            throw new ArgumentException("Contract id template can't be empty");
        
        ContractTemplateId = contractTemplateId;
        Client = client ?? throw new ArgumentNullException(nameof(client));
        Car = car ?? throw new ArgumentNullException(nameof(car));
        Template = template ?? throw new ArgumentNullException(nameof(template));
        Rental = rental  ?? throw new ArgumentNullException(nameof(rental));
        Status = ContractStatus.AwaitingSignature;
        AddDomainEvent(new ContractCreatedDomainEvent(Id, DateTime.UtcNow));
    }
    
    public void Sign()
    {
        if(Status != ContractStatus.AwaitingSignature)
            throw new InvalidOperationException("Contract status must be AwaitingSignature");
        
        Status = ContractStatus.Active;
        AddDomainEvent(new ContractSignedDomainEvent(Id, Status, DateTime.UtcNow));
    }

    public void EndContract()
    {
        if(Status != ContractStatus.Active)
            throw new InvalidOperationException("Contract status must be Active");
        
        Status = ContractStatus.Ended;
        AddDomainEvent(new ContractEndedDomainEvent(Id, Status, DateTime.UtcNow));
    }
}