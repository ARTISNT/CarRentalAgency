namespace RentalService.Application.Features.Rentals.CreateRental;

public class UserRentInfoResponse
{
    public string Name { get; set; }
    public string SurName { get; set; }
    public string Patronymic  { get; set; }
    public string PhoneNumber { get; set; }
    public string Email  { get; set; }
    public bool HasPassport { get; set; }
}