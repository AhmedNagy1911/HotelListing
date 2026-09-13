using AutoMapper;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Domain;

namespace HotelListing.Api.Application.MappingProfiles;

public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        CreateMap<Hotel, GetHotelDto>()
           // .ForMember(d => d.Country, cfg => cfg.MapFrom(src => src.Country != null ? src.Country.Name : string.Empty));
           .ForCtorParam("Country", opt => opt.MapFrom(src => src.Country != null ? src.Country.Name : string.Empty));
        CreateMap<Hotel, GetHotelSlimDto>(); 
        CreateMap<CreateHotelDto, Hotel>();
    }
}
