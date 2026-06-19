using ContractService.Domain.Common;
using ContractService.Domain.DomainEvents;

namespace ContractService.Domain.Contracts;

public sealed class Contract : Entity, IAggregateRoot
{
    public Guid ContractTemplateId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid RentalId { get; private set; }
    public Guid ContractAutoId { get; private set; }
    public string? CancellationReason { get; private set; } 
    public IReadOnlyCollection<ContractAddition> ContractAdditions => _contractAdditions.AsReadOnly();
    public ContractReturnAct? ReturnAct { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ContractStatus Status { get; private set; }
    public ClientSnapshot Client { get; }
    public ContractAutoSnapshot Car { get; }
    public ContractTemplateSnapshot Template { get; }
    public RentalSnapshot Rental { get; }
    
    public DateTime ActualEndDate =>
        _contractAdditions.Any()
            ? _contractAdditions.Last().NewEndDate
            : Rental.EndDate; 
    
    private readonly List<ContractAddition> _contractAdditions = new ();
    
    private Contract() {}
    
    public Contract(Guid contractTemplateId,
        Guid contractAutoId,
        Guid clientId,
        Guid rentalId,
        ClientSnapshot client,
        ContractAutoSnapshot car,
        ContractTemplateSnapshot template,
        RentalSnapshot rental)
    {
        if(template.DocumentType != DocumentType.Contract.Name)
            throw new ArgumentException("Contract template document type must be Contract");
            
        ValidateIdentifiers(contractTemplateId,
            clientId,
            rentalId,
            contractAutoId);
        
        ContractTemplateId = contractTemplateId;
        RentalId = rentalId;
        ContractAutoId = contractAutoId;
        ClientId = clientId;
        
        Client = client ?? throw new ArgumentNullException(nameof(client));
        Car = car ?? throw new ArgumentNullException(nameof(car));
        Template = template ?? throw new ArgumentNullException(nameof(template));
        Rental = rental  ?? throw new ArgumentNullException(nameof(rental));
        CreatedAt = DateTime.UtcNow;
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

    public void RenewContract(
        DateTime newEndDate,
        decimal additionalCost,
        ContractTemplateSnapshot template)
    {
        ArgumentNullException.ThrowIfNull(template);

        if (Status != ContractStatus.Active)
            throw new InvalidOperationException(
                "Contract status must be Active");

        var addition = new ContractAddition(
            ActualEndDate,
            newEndDate,
            additionalCost,
            template);

        _contractAdditions.Add(addition);

        AddDomainEvent(
            new ContractRenewedDomainEvent(
                Id,
                DateTime.UtcNow));
    }

    public void EndContract(ContractReturnAct contractReturnAct)
    {
        if(Status != ContractStatus.Active)
            throw new InvalidOperationException("Contract status must be Active");
        
        Status = ContractStatus.Ended;
        ReturnAct = contractReturnAct ?? throw new ArgumentNullException(nameof(contractReturnAct));
        AddDomainEvent(new ContractEndedDomainEvent(Id, Status, DateTime.UtcNow));
    }
    
    public void Cancel(string reason)
    {
        if (Status != ContractStatus.AwaitingSignature)
            throw new InvalidOperationException("Contract can be cancelled only until it signed");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required");

        Status = ContractStatus.Cancelled;
        CancellationReason = reason;

        AddDomainEvent(new ContractCancelledDomainEvent(
            Id,
            reason,
            DateTime.UtcNow));
    }

    public void AddTerminationAddition(ContractAddition addition)
    {
        ArgumentNullException.ThrowIfNull(addition);
        _contractAdditions.Add(addition);
    }
    
    private static void ValidateIdentifiers(Guid contractTemplateId,
        Guid clientId, Guid rentalId, Guid contractAutoId)
    {
        if(contractTemplateId == Guid.Empty)
            throw new ArgumentException("Contract template id can't be empty");
        
        if(rentalId == Guid.Empty)
            throw new ArgumentException("Rental id can't be empty");
        
        if(contractTemplateId  == Guid.Empty)
            throw new ArgumentException("Contract id template can't be empty");
        
        if(clientId == Guid.Empty)
            throw new ArgumentException("User id can't be empty");
    }
}