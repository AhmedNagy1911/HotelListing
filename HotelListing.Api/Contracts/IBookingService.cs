using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.Results;

namespace HotelListing.Api.Contracts;

public interface IBookingService
{
    Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId);
    Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto dto);
    Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto dto);
    Task<Result> CancelBookingAsync(int hotelId, int bookingId)
}
