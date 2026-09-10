using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(HotelListingDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var countries = await context.Countries
            .Select(c => new GetCountriesDto(c.Id, c.Name, c.ShortName))
            .ToListAsync();
        
        return Ok(countries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
        var country = await context.Countries
            .Where(c => c.Id == id)
            .Select(c =>  new GetCountryDto(
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

        if (country == null)
            return NotFound();

        return Ok(country);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto countryDto)
    {
        if (id != countryDto.Id)
            return BadRequest();

        var country = await context.Countries.FindAsync(id);

        if (country is null)
            return NotFound();

        country.Name = countryDto.Name;
        country.ShortName = countryDto.ShortName;

        context.Entry(country).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await CountryExistsAsync(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
    {
        var country = new Country
        {
            Name = createCountryDto.Name,
            ShortName = createCountryDto.ShortName
        };

        context.Countries.Add(country);
        await context.SaveChangesAsync();

        var resultDto = new GetCountryDto(
            country.Id,
            country.Name,
            country.ShortName,
            []
        );

        return CreatedAtAction("GetCountry", new { id = country.Id }, resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await context.Countries.FindAsync(id);

        if (country == null)
            return NotFound();

        context.Countries.Remove(country);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> CountryExistsAsync(int id)
    {
        return await context.Countries.AnyAsync(e => e.Id == id);
    }
}