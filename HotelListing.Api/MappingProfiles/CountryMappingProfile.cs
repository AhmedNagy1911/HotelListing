using AutoMapper;
using HotelListing.Api.Domain;
using HotelListing.Api.DTOs.Country;

namespace HotelListing.Api.MappingProfiles;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountryDto>();
        CreateMap<Country, GetCountriesDto>();
        CreateMap<CreateCountryDto, Country>();
    }
}
