using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;

namespace HotelListing.Api.MappingProfiles;

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
public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountryDto>();
        CreateMap<Country, GetCountriesDto>();
        CreateMap<CreateCountryDto, Country>();
    }
}
