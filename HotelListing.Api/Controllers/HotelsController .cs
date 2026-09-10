using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(HotelListingDbContext context) : ControllerBase
{
    private readonly HotelListingDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetHotelsDto>>> GetHotels()
    {
        var hotels = await _context.Hotels
            .Select(x => new GetHotelsDto( x.Id , x.Name, x.Address, x.Rating, x.CountryId))
            .ToListAsync();

        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetHotelDto>> GetHotel(int id)
    {
        var hotel = await _context.Hotels
            .Where(x => x.Id == id)
            .Select(x => new GetHotelDto(x.Id, x.Name, x.Address, x.Rating, x.Country!.Name))
            .FirstOrDefaultAsync();

        if (hotel is null)
            return NotFound();

        return hotel;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        if (id != hotelDto.Id)
            return BadRequest();

        var hotel = await _context.Hotels.FindAsync(id);

        if (hotel is null)
            return NotFound();

        hotel.Name = hotelDto.Name;
        hotel.Address = hotelDto.Address;
        hotel.Rating = hotelDto.Rating;
        hotel.CountryId = hotelDto.CountryId;

        _context.Entry(hotelDto).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!HotelExists(id))
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
    public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto hotelDto)
    {
        var hotel = new Hotel
        {
            Name = hotelDto.Name,
            Address = hotelDto.Address,
            Rating = hotelDto.Rating,
            CountryId = hotelDto.CountryId
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
            return NotFound();

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool HotelExists(int id)
    {
        return _context.Hotels.Any(e => e.Id == id);
    }
}