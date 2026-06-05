using AutoMapper;
using ContractService.Application.Features.Contracts.CreateContract;
using ContractService.Application.Features.Contracts.GetContract;
using ContractService.Application.Features.Contracts.GetContracts;
using ContractService.Application.Features.Contracts.GetDetailedContract;
using ContractService.Application.Features.ContractsTemplates.GetContractTemplates;
using ContractService.Domain.Contracts;

namespace ContractService.Application.MappingResponse;

public class ContractMappingResponseProfile : Profile
{
    public ContractMappingResponseProfile()
    {
        CreateMap<ClientForContractResponse, ClientSnapshot>();
        CreateMap<CarForContractResponse, ContractAutoSnapshot>();
        CreateMap<RentalForContractResponse, RentalSnapshot>(); 
        
        CreateMap<ContractTemplate, ContractTemplateSnapshot>()
            .ForMember(dest => dest.DocumentType,
                opt => opt.MapFrom(src => src.DocumentType.Name));
        
        CreateMap<Contract, ContractListResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(src =>
                $"{src.Client.Name} {src.Client.Surname}".Trim()))
            .ForMember(dest => dest.Car, opt => opt.MapFrom(src =>
                $"{src.Car.Brand} {src.Car.Model} ({src.Car.LicensePlate})"))
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.Rental.StartDate))
            .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.Rental.EndDate))
            .ForMember(dest => dest.EstimatedPrice, opt => opt.MapFrom(src => src.Rental.EstimatedPrice))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
            .ForMember(dest => dest.PdfPath, opt => opt.Ignore()); 
        
        CreateMap<Contract, ContractResponse>()
            .IncludeMembers(src => src.Client);

        CreateMap<ClientSnapshot, ContractResponse>(); 
        
        CreateMap<Contract, DetailedContractResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ContractTemplateId, opt => opt.MapFrom(src => src.ContractTemplateId))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
            .ForMember(dest => dest.PdfPath, opt => opt.Ignore());

        CreateMap<ClientSnapshot, ClientResponse>();
        CreateMap<ContractAutoSnapshot, ContractAutoResponse>();
        CreateMap<ContractTemplateSnapshot, ContractTemplateResponse>();
        CreateMap<ContractTemplate, ContractTemplateListResponse>();
        CreateMap<RentalSnapshot, RentalResponse>(); 
    }
}