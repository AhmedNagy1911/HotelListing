namespace HotelListing.Api.DTOs.Country;

public record GetCountriesDto(
    int Id,
    string Name,
    string ShortName
);
