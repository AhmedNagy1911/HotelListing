using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController(ICountriesService countriesService) : ControllerBase
{
    private readonly ICountriesService _countriesService = countriesService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
    {
        var countries = await _countriesService.GetCountriesAsync();

        return Ok(countries);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
    {
        var country = await _countriesService.GetCountryAsync(id);

        if (country == null)
            return NotFound();

        return Ok(country);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCountry(int id, UpdateCountryDto countryDto)
    {
        if (id != countryDto.Id)
            return BadRequest();

        await _countriesService.UpdateCountryAsync(id, countryDto);

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createCountryDto)
    {
        var resultDto = await _countriesService.CreateCountryAsync(createCountryDto);

        return CreatedAtAction("GetCountry", new { id = resultDto.Id }, resultDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        await _countriesService.DeleteCountryAsync(id);

        return NoContent();
    }
}