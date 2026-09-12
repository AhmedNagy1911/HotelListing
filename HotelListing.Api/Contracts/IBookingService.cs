using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts;

public interface IBookingService
{
    Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId);
}
