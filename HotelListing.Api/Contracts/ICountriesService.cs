using HotelListing.Api.Common.Results;
using HotelListing.Api.DTOs.Country;

namespace HotelListing.Api.Contracts;

public interface ICountriesService
{
    Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync();
    Task<Result<GetCountryDto>> GetCountryAsync(int id);
    Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto createCountryDto);
    Task<Result> UpdateCountryAsync(int id, UpdateCountryDto updateCountryDto);
    Task<Result> DeleteCountryAsync(int id);
    Task<bool> CountryExistsAsync(int id);
    Task<bool> CountryExistsAsync(string name);
}
