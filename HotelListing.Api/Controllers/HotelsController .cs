using HotelListing.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(HotelListingDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels()
    {
        return await context.Hotels
            //.Include(h => h.Country) // Eager loading the Country navigation property
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Hotel>> GetHotel(int id)
    {
        var hotel = await context.Hotels
            .Include(h => h.Country) 
            .FirstOrDefaultAsync(q => q.Id == id);

        if (hotel == null)
            return NotFound();

        return hotel;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, Hotel hotel)
    {
        if (id != hotel.Id)
            return BadRequest();

        context.Entry(hotel).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
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
    public async Task<ActionResult<Hotel>> PostHotel(Hotel hotel)
    {
        context.Hotels.Add(hotel);
        await context.SaveChangesAsync();

        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var hotel = await context.Hotels.FindAsync(id);
        if (hotel == null)
            return NotFound();

        context.Hotels.Remove(hotel);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool HotelExists(int id)
    {
        return context.Hotels.Any(e => e.Id == id);
    }
}