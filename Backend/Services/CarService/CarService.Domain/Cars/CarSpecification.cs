namespace CarService.Domain.Cars;

public class CarSpecification
{
    public string? Status { get; set; }
    public string? Class { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? RentedBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
