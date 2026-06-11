namespace ContractService.Domain.Contracts;

public class ContractSpecification
{
    public Guid? ClientId { get; set; }
    public Guid? RentalId { get; set; }
    public string? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}