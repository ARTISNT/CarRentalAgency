using CarService.Application.Features.GetCars;

namespace CarService.Application.Features.GetDetailedCars;

public class PublicCarDetailsResponse : CarListResponse
{
    public DateTime ReleaseDate { get; set; }
    public double Mileage { get; set; }
    public string Transmission { get; set; }
    public string DriveType { get; set; }
}
