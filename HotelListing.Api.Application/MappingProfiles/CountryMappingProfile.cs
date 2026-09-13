using AutoMapper;
using HotelListing.Api.Application.DTOs.Country;
using HotelListing.Api.Domain;

namespace HotelListing.Api.Application.MappingProfiles;

public class CountryMappingProfile : Profile
{
    public CountryMappingProfile()
    {
        CreateMap<Country, GetCountryDto>();
        CreateMap<Country, GetCountriesDto>();
        CreateMap<CreateCountryDto, Country>();
    }
}
