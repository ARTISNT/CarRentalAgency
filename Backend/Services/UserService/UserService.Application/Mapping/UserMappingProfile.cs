using AutoMapper;
using UserService.Application.Features.Users.AddUserPassport;
using UserService.Application.Features.Users.GetUserForRent;
using UserService.Application.Features.Users.GetUserPersonal;
using UserService.Application.Features.Users.GetUsers;
using UserService.Domain.Users;

namespace UserService.Application.Mapping;
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Password,
                opt => opt.MapFrom(src => src.Password.Hash))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber.Value));

        CreateMap<User, UserResponseWithPassport>()
            .IncludeBase<User, UserResponse>()
            .ForMember(dest => dest.PassportDto,
                opt => opt.MapFrom(src => src.Passport));

        CreateMap<Passport, PassportDto>();

        CreateMap<PassportNumber, string>()
            .ConvertUsing(src => src.Value);

        CreateMap<IdentityNumber, string>()
            .ConvertUsing(src => src.Value);

        CreateMap<User, UserRentInfoResponse>()
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Passport.Name))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.Email.Value))
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => src.PhoneNumber.Value))
            .ForMember(dest => dest.SurName,
                opt => opt.MapFrom(src => src.Passport.Surname))
            .ForMember(dest => dest.Patronymic,
                opt => opt.MapFrom(src => src.Passport.Patronymic));

    }
}
