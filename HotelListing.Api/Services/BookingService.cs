using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Data.Enums;
using HotelListing.Api.DTOs.Booking;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace HotelListing.Api.Services;

public class BookingService(HotelListingDbContext context , IHttpContextAccessor httpContextAccessor) : IBookingService
{
    private readonly HotelListingDbContext _context = context;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<Result<IEnumerable<GetBookingDto>>> GetBookingsForHotelAsync (int hotelId)
    {
        var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == hotelId);
        if (!hotelExists)
          return Result<IEnumerable<GetBookingDto>>.Failure(new Error(ErrorCodes.NotFound, $"Hotel '{hotelId}' was not found."));

        var bookings = await _context.Bookings
            .Where(b => b.HotelId == hotelId)
            .OrderBy(b => b.CheckIn)
            .Select(b => new GetBookingDto(
                b.Id,
                b.HotelId,
                b.Hotel!.Name,
                b.CheckIn,
                b.CheckOut,
                b.Guests,
                b.TotalPrice,
                b.Status.ToString(),
                b.CreatedAtUtc,
                b.UpdatedAtUtc
            ))
            .ToListAsync();

        return Result<IEnumerable<GetBookingDto>>.Success(bookings);
    }

    public async Task<Result<GetBookingDto>> CreateBookingAsync(CreateBookingDto dto)
    {
        var userId = _httpContextAccessor?.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if(string.IsNullOrEmpty(userId))
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "User is not authenticated."));

        var nights = dto.CheckOut.DayNumber - dto.CheckIn.DayNumber;
        if (nights <= 0)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Validation, "Check-out date must be after check-in date."));

        var hotel = await context.Hotels
           .Where(h => h.Id == dto.HotelId)
           .FirstOrDefaultAsync();

        if (hotel is null)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.NotFound, $"Hotel '{dto.HotelId}' was not found."));
       
        var overlaps = await context.Bookings.AnyAsync(
            b => b.HotelId == dto.HotelId
                 && b.Status != BookingStatus.Cancelled 
                 && dto.CheckIn < b.CheckOut
                 && dto.CheckOut > b.CheckIn
                 && b.UserId == userId
        );

        if (overlaps)
            return Result<GetBookingDto>.Failure(new Error(ErrorCodes.Conflict, "The selected dates overlap with an existing booking."));
       
        var totalPrice = hotel.PerNightRate * nights;

        var booking = new Booking
        {
            HotelId = dto.HotelId,
            UserId = userId,
            CheckIn = dto.CheckIn,
            CheckOut = dto.CheckOut,
            Guests = dto.Guests,
            TotalPrice = totalPrice,
            Status = BookingStatus.Pending,
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var created = new GetBookingDto(
            booking.Id,
            hotel.Id,
            hotel.Name,
            dto.CheckIn,
            dto.CheckOut,
            dto.Guests,
            totalPrice,
            BookingStatus.Pending.ToString(),
            booking.CreatedAtUtc,
            booking.UpdatedAtUtc
        );

        return Result<GetBookingDto>.Success(created);
    }
}
