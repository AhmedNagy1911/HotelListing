namespace HotelListing.Api.Application.DTOs.Country;

public record GetCountriesDto(
    int Id,
    string Name,
    string ShortName
);
