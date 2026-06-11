using AutoMapper;
using RentalService.Application.Features.Rentals.GetRental;
using RentalService.Application.Features.Rentals.GetRentalForContract;
using RentalService.Application.Features.Rentals.GetRentals;
using RentalService.Domain.Rentals;

namespace RentalService.Application.Mapping;
public class RentalProfile : Profile
{
    public RentalProfile()
    {
        CreateMap<RentCarSnapshot, RentalCarResponse>();

        CreateMap<CarRenterSnapshot, RentalRenterResponse>();

        CreateMap<Rental, RentalResponse>()
            .ForMember(
                dest => dest.Car,
                opt => opt.MapFrom(src => src.RentCarSnapshot))
            
            .ForMember(
                dest => dest.Renter,
                opt => opt.MapFrom(src => src.CarRenterSnapshot))
            
            .ForMember(
                dest => dest.TotalCost,
                opt => opt.Ignore())
            
            .AfterMap((src, dest) => dest.Car.Id = src.RentCarId);

        CreateMap<Rental, RentalListResponseDto>()
            .ForMember(
                dest => dest.Car,
                opt => opt.MapFrom(src =>
                    $"{src.RentCarSnapshot.Brand} {src.RentCarSnapshot.Model}"))

            .ForMember(
                dest => dest.Renter,
                opt => opt.MapFrom(src =>
                    $"{src.CarRenterSnapshot.SurName} {src.CarRenterSnapshot.Name}"))

            .ForMember(
                dest => dest.RenterId,
                opt => opt.MapFrom(src => src.CarRenterId))

            .ForMember(
                dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.CarRenterSnapshot.PhoneNumber));
    }
}
