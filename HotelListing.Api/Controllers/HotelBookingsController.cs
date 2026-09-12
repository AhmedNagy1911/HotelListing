using HotelListing.Api.Contracts;
using HotelListing.Api.DTOs.Booking;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[Route("api/hotels/{hotelId:int}/bookings")]
[ApiController]
[Authorize]
public class HotelBookingsController(IBookingService bookingService) : BaseApiController
{
    private readonly IBookingService _bookingService = bookingService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetBookingDto>>> GetBookings([FromRoute] int hotelId)
    {
        var result = await _bookingService.GetBookingsForHotelAsync(hotelId);

        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<GetBookingDto>> CreateBooking([FromRoute] int hotelId,[FromBody] CreateBookingDto createBookingDto)
    {
        var result = await _bookingService.CreateBookingAsync(createBookingDto);

        return ToActionResult(result);
    }
}
