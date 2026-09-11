using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class HotelsService(HotelListingDbContext context, IMapper mapper) : IHotelsService
{
    private readonly HotelListingDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<GetHotelDto>> GetHotelsAsync()
    {
        var hotels = await _context.Hotels
                 .ProjectTo<GetHotelDto>(_mapper.ConfigurationProvider)
                 .ToListAsync();

        return hotels;
    }

    public async Task<GetHotelDto?> GetHotelAsync(int id)
    {
        var hotel = await _context.Hotels
            .Where(x => x.Id == id)
            .ProjectTo<GetHotelDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return hotel ?? null;
    }
    public async Task<GetHotelDto> CreateHotelAsync(CreateHotelDto hotelDto)
    {
        var hotel = _mapper.Map<Hotel>(hotelDto);
        
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<GetHotelDto>(hotel);

        return result;
    }

    public async Task UpdateHotelAsync(int id, UpdateHotelDto updateDto)
    {
        var hotel = await _context.Hotels.FindAsync(id) ?? throw new KeyNotFoundException($"Country with ID {id} not found.");

       _mapper.Map(updateDto, hotel);

        _context.Entry(hotel).State = EntityState.Modified;
         _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync();

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
