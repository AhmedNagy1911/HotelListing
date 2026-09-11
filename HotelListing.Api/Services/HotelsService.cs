using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class HotelsService(HotelListingDbContext context,
    ICountriesService countriesService,
    IMapper mapper) : IHotelsService
{
    private readonly HotelListingDbContext _context = context;
    private readonly ICountriesService _countriesService = countriesService;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync()
    {
        var hotels = await _context.Hotels
            .ProjectTo<GetHotelDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<IEnumerable<GetHotelDto>>.Success(hotels);
    }

    public async Task<Result<GetHotelDto>> GetHotelAsync(int id)
    {
        var hotel = await _context.Hotels
            .Where(h => h.Id == id)
            .ProjectTo<GetHotelDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        if (hotel is null)
            return Result<GetHotelDto>.Failure(new Error(ErrorCodes.NotFound, $"Hotel '{id}' was not found."));

        return Result<GetHotelDto>.Success(hotel);
    }

    public async Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hotelDto)
    {
        var countryExists = await _countriesService.CountryExistsAsync(hotelDto.CountryId);
        if (!countryExists)
            return Result<GetHotelDto>.Failure(new Error(ErrorCodes.NotFound, $"Country '{hotelDto.CountryId}' was not found."));

        var duplicate = await HotelExistsAsync(hotelDto.Name, hotelDto.CountryId);
        if (duplicate)
        {
            return Result<GetHotelDto>.Failure
                (new Error(ErrorCodes.Conflict, $"Hotel '{hotelDto.Name}' already exists in the selected country."));
        }

        var hotel = _mapper.Map<Hotel>(hotelDto);
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        var dto = await _context.Hotels
            .Where(h => h.Id == hotel.Id)
            .ProjectTo<GetHotelDto>(_mapper.ConfigurationProvider)
            .FirstAsync();

        return Result<GetHotelDto>.Success(dto);
    }

    public async Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateDto)
    {
        if (id != updateDto.Id)
            return Result.BadRequest(new Error(ErrorCodes.Validation, "Id route value does not match payload Id."));

        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel is null)
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Hotel '{id}' was not found."));

        var countryExists = await _countriesService.CountryExistsAsync(updateDto.CountryId);
        if (!countryExists)
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{updateDto.CountryId}' was not found."));
 
        _mapper.Map(updateDto, hotel);

        _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    public async Task<Result> DeleteHotelAsync(int id)
    {
        var affected = await _context.Hotels
            .Where(q => q.Id == id)
            .ExecuteDeleteAsync();

        if (affected == 0)
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Hotel '{id}' was not found."));

        return Result.Success();
    }

    public async Task<bool> HotelExistsAsync(int id)
    {
        return await _context.Hotels.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> HotelExistsAsync(string name, int countryId)
    {
        return await _context.Hotels
            .AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim() && e.CountryId == countryId);
    }
}