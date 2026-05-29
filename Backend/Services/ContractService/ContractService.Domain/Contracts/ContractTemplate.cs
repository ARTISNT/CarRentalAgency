using ContractService.Domain.Common;
using ContractService.Domain.DomainEvents;

namespace ContractService.Domain.Contracts;

public sealed class ContractTemplate : Entity
{
    public int Version { get; private set; }
    public string Name { get; private set; }
    public string Content { get; private set; }
    public DateTime ValidFrom { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public bool IsActive { get; private set; }

    private const int MaxNameLength = 50;
    private const int MinNameLength = 3;

    private ContractTemplate() { } 

    public ContractTemplate(
        string name,
        string content,
        DateTime validFrom, 
        int version)
    {
        SetName(name);
        SetContent(content);

        if (validFrom == default)
            throw new ArgumentException("Invalid valid from date");
        
        if(version <= 0)
            throw new ArgumentException("Version must be greater than 0");

        ValidFrom = validFrom;
        Version = version;

        CreatedOn = DateTime.UtcNow;
        IsActive = true;
        AddDomainEvent(new ContractTemplateCreatedDomainEvent(Id, DateTime.UtcNow));
    }

    public void Activate()
    {
        if(IsActive)
            return;
        
        IsActive = true;   
        AddDomainEvent(new ContractTemplateActivatedDomainEvent(Id, IsActive, DateTime.UtcNow));
    }

    public void Deactivate()
    {
        if(!IsActive)
            return;
        
        IsActive = false;   
        AddDomainEvent(new ContractTemplateDeactivatedDomainEvent(Id, IsActive, DateTime.UtcNow));
    }

    public void UpdateContent(string content)
    {
        SetContent(content);
        AddDomainEvent(new ContractTemplateContentUpdatedDomainEvent(Id, content, DateTime.UtcNow));
    }

    public void Rename(string name)
    {
        SetName(name);
        AddDomainEvent(new ContractTemplateRenamedDomainEvent(Id, name, DateTime.UtcNow));
    }

    private void SetContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException(
                "Content cannot be empty",
                nameof(content));

        Content = content;
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Name cannot be empty",
                nameof(name));

        if (name.Length < MinNameLength ||
            name.Length > MaxNameLength)
        {
            throw new ArgumentOutOfRangeException(
                nameof(name),
                $"Name length must be between {MinNameLength} and {MaxNameLength}");
        }

        Name = name;
    }
}
