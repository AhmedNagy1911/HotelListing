using HotelListing.Api.Common.Results;
using HotelListing.Api.DTOs.Booking;

namespace HotelListing.Api.Contracts;

public interface IBookingService
{
    Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync(int hotelId);
    Task<Result<IEnumerable<GetBookingDto>>> GetUserBookingsForHotelAsync(int hotelId);
    Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto dto);
    Task<Result<GetBookingDto>> UpdateBookingAsync(int hotelId, int bookingId, UpdateBookingDto dto);
    Task<Result> CancelBookingAsync(int hotelId, int bookingId);
    Task<Result> AdminCancelBookingAsync(int hotelId, int bookingId);
    Task<Result> AdminConfirmBookingAsync(int hotelId, int bookingId);
}
