using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(IHotelsService hotelsService) : ControllerBase
{
    private readonly IHotelsService _hotelsService = hotelsService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotels()
    {
        var hotels = await _hotelsService.GetHotelsAsync();

        return Ok(hotels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetHotelDto>> GetHotel(int id)
    {
        var hotel = await _hotelsService.GetHotelAsync(id);

        if (hotel is null)
            return NotFound();

        return hotel;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        if (id != hotelDto.Id)
            return BadRequest();

        try
        {
            await _hotelsService.UpdateHotelAsync(id, hotelDto);
        }
        catch (DbUpdateConcurrencyException)
        {
           if(!await _hotelsService.HotelExistsAsync(id))
                return NotFound();
            else
                throw;
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Hotel>> PostHotel(CreateHotelDto hotelDto)
    {
       var hotel = await _hotelsService.CreateHotelAsync(hotelDto);

        return CreatedAtAction("GetHotel", new { id = hotel.Id }, hotel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        await _hotelsService.DeleteHotelAsync(id);

        return NoContent();
    }

}