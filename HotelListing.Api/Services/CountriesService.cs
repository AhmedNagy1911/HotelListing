using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class CountriesService(HotelListingDbContext context) : ICountriesService
{
    private readonly HotelListingDbContext _context = context;

    public async Task<IEnumerable<GetCountriesDto>> GetCountriesAsync()
    {
        var countries = await _context.Countries
            .Select(c => new GetCountriesDto(c.Id, c.Name, c.ShortName))
            .ToListAsync();

        return countries;
    }

    public async Task<GetCountryDto?> GetCountryAsync(int id)
    {
        var country = await _context.Countries
            .Where(c => c.Id == id)
            .Select(c => new GetCountryDto(
                c.Id,
                c.Name,
                c.ShortName,
                c.Hotels.Select(h => new GetHotelSlimDto(
                    h.Id,
                    h.Name,
                    h.Address,
                    h.Rating
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        return country;
    }

    public async Task<GetCountryDto> CreateCountryAsync(CreateCountryDto createCountryDto)
    {
        var country = new Country
        {
            Name = createCountryDto.Name,
            ShortName = createCountryDto.ShortName
        };

        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        var resultDto = new GetCountryDto(
            country.Id,
            country.Name,
            country.ShortName,
            []
        );

        return resultDto;
    }

    public async Task UpdateCountryAsync(int id, UpdateCountryDto updateCountryDto)
    {

        var country = await _context.Countries.FindAsync(id) ?? throw new KeyNotFoundException($"Country with ID {id} not found.");
        
        country.Name = updateCountryDto.Name;
        country.ShortName = updateCountryDto.ShortName;

        _context.Countries.Update(country);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCountryAsync(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country is null)
        {
            throw new KeyNotFoundException($"Country with ID {id} not found.");
        }
        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CountryExistsAsync(int id)
    {
        return await _context.Countries.AnyAsync(e => e.Id == id);
    }
    public async Task<bool> CountryExistsAsync(string name)
    {
        return await _context.Countries.AnyAsync(e => e.Name == name);
    }
}
