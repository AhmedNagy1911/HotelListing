using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Constants;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Country;
using HotelListing.Api.DTOs.Hotel;
using HotelListing.Api.Results;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class CountriesService(HotelListingDbContext context , IMapper mapper) : ICountriesService
{
    private readonly HotelListingDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync()
    {
        var countries = await _context.Countries
            .Select(c => new GetCountriesDto(c.Id, c.Name, c.ShortName))
            .ToListAsync();

        return Result<IEnumerable<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {
        var country = await _context.Countries
            .Where(c => c.Id == id)
            .Select(c => new GetCountryDto(
                c.Id,
                c.Name,
                c.ShortName,
                c.Hotels.Select(h => new GetHotelSlimDto(
                    h.Id,
                    h.Name,
                    h.Address,
                    h.Rating
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        return country is null 
            ? Result<GetCountryDto>.NotFound() 
            : Result<GetCountryDto>.Success(country);
    }

    public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto createDto)
    {
        try
        {
            var exists = await CountryExistsAsync(createDto.Name);
            if (exists)
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, $"Country with name '{createDto.Name}' already exists."));

            var country = _mapper.Map<Country>(createDto);

            _context.Countries.Add(country);
            await _context.SaveChangesAsync();

            var dto = await _context.Countries
                .Where(c => c.Id == country.Id)
                .ProjectTo<GetCountryDto>(_mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<GetCountryDto>.Success(dto);
        }
        catch
        {
            return Result<GetCountryDto>.Failure
                (new Error(ErrorCodes.Failure, "An unexpected error occurred while creating the country."));
        }
    }

    public async Task<Result> UpdateCountryAsync(int id, UpdateCountryDto updateDto)
    {

        try
        {
            if (id != updateDto.Id)
                return Result.BadRequest(new Error(ErrorCodes.Validation, "Id route value does not match payload Id."));

            var country = await _context.Countries.FindAsync(id);
            if (country is null)
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."));

            _mapper.Map(updateDto, country);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch
        {
            return Result.Failure
                (new Error(ErrorCodes.Failure, "An unexpected error occurred while updating the country."));
        }
    }

    public async Task<Result> DeleteCountryAsync(int id)
    {
        try
        {
            var country = await _context.Countries.FindAsync(id);
            if (country is null)
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."));

            _context.Countries.Remove(country);
            await _context.SaveChangesAsync();

            return Result.Success();
        }
        catch
        {
            return Result.Failure(new Error(ErrorCodes.Failure, "An unexpected error occurred while deleting the country."));
        }
    }

    public async Task<bool> CountryExistsAsync(int id)
    {
        return await _context.Countries.AnyAsync(e => e.Id == id);
    }
    public async Task<bool> CountryExistsAsync(string name)
    {
        return await _context.Countries.AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }
}
