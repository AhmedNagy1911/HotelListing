using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class HotelsService(HotelListingDbContext context) : IHotelsService
{
    private readonly HotelListingDbContext _context = context;

    public async Task<IEnumerable<GetHotelDto>> GetHotelsAsync()
    {
        var hotels = await _context.Hotels
                  .Include(x => x.Country)
                 .Select(x => new GetHotelDto(x.Id, x.Name, x.Address, x.Rating, x.CountryId,x.Country!.Name))
                 .ToListAsync();

        return hotels;
    }

    public async Task<GetHotelDto?> GetHotelAsync(int id)
    {
        var hotel = await _context.Hotels
            .Where(x => x.Id == id)
            .Select(x => new GetHotelDto(x.Id, x.Name, x.Address, x.Rating, x.CountryId, x.Country!.Name))
            .FirstOrDefaultAsync();

        return hotel ?? null;
    }

    public async Task UpdateHotelAsync(int id, UpdateHotelDto updateDto)
    {
        var hotel = await _context.Hotels.FindAsync(id) ?? throw new KeyNotFoundException($"Country with ID {id} not found.");

        hotel.Name = updateDto.Name;
        hotel.Address = updateDto.Address;
        hotel.Rating = updateDto.Rating;
        hotel.CountryId = updateDto.CountryId;

        _context.Entry(hotel).State = EntityState.Modified;
         _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync();

    }
    public async Task<GetHotelDto> CreateHotelAsync(CreateHotelDto hotelDto)
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

        return new GetHotelDto(hotel.Id, hotel.Name, hotel.Address, hotel.Rating, hotel.CountryId,hotel.Country?.Name ?? string.Empty);
    }

    public async Task DeleteHotelAsync(int id)
    {
        var hotel = await _context.Hotels
            .Where(x => x.Id == id)
            .ExecuteDeleteAsync();
    }

    public async Task<bool> HotelExistsAsync(int id)
    {
        return await _context.Hotels.AnyAsync(x => x.Id == id);
    }

    public async Task<bool> HotelExistsAsync(string name, int countryId)
    {
        return await _context.Hotels.AnyAsync(x => x.Name == name && x.CountryId == countryId);
    }

   
}
