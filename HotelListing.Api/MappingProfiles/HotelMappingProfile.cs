using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Booking;
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

public sealed class BookingMappingProfile : Profile
{
    public BookingMappingProfile()
    {
        CreateMap<Booking, GetBookingDto>()
            .ForMember(d => d.HotelName, o => o.MapFrom(s => s.Hotel!.Name))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<CreateBookingDto, Booking>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.TotalPrice, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore())
            .ForMember(d => d.CreatedAtUtc, o => o.Ignore())
            .ForMember(d => d.UpdatedAtUtc, o => o.Ignore())
            .ForMember(d => d.Hotel, o => o.Ignore());

        CreateMap<UpdateBookingDto, Booking>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.TotalPrice, o => o.Ignore())
            .ForMember(d => d.Status, o => o.Ignore())
            .ForMember(d => d.CreatedAtUtc, o => o.Ignore())
            .ForMember(d => d.UpdatedAtUtc, o => o.Ignore())
            .ForMember(d => d.Hotel, o => o.Ignore());
    }
}