using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Country;
using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Models.Extensions;
using HotelListing.Api.Common.Models.Filtering;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Domain;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Application.Services;

public class CountriesService(HotelListingDbContext context, IMapper mapper) : ICountriesService
{
    private readonly HotelListingDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync(CountryFilterParameters filters)
    {
        var query = _context.Countries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var term = filters.Search.Trim();
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{term}%")
            || EF.Functions.Like(c.ShortName, $"%{term}%"));
        }

        var countries = await query
            .AsNoTracking()
            .ProjectTo<GetCountriesDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Result<IEnumerable<GetCountriesDto>>.Success(countries);
    }

    public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
    {

        var country = await _context.Countries
            .AsNoTracking()
            .Where(q => q.Id == id)
            .ProjectTo<GetCountryDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return country is null
            ? Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."))
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

            var dto = _mapper.Map<GetCountryDto>(country);

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
        return await _context.Countries.AsNoTracking().AnyAsync(e => e.Id == id);
    }
    public async Task<bool> CountryExistsAsync(string name)
    {
        return await _context.Countries.AsNoTracking().AnyAsync(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public async Task<Result<GetCountryHotelsDto>> GetCountryHotelsAsync(int countryId, PaginationParameters paginationParameters, CountryFilterParameters filters)
    {
        var exists = await CountryExistsAsync(countryId);
        if (!exists)
        {
            return Result<GetCountryHotelsDto>.Failure(
                new Error(ErrorCodes.NotFound, $"Country '{countryId}' was not found."));
        }

        var countryName = await _context.Countries
            .Where(q => q.Id == countryId)
            .Select(q => q.Name)
            .SingleAsync();

        var hotelsQuery = _context.Hotels
            .Where(h => h.CountryId == countryId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Search))
        {
            var term = filters.Search.Trim();
            hotelsQuery = hotelsQuery.Where(h => EF.Functions.Like(h.Name, $"%{term}%"));
        }

        hotelsQuery = (filters.SortBy?.Trim().ToLowerInvariant()) switch
        {
            "name" => filters.SortDescending ? hotelsQuery.OrderByDescending(h => h.Name) : hotelsQuery.OrderBy(h => h.Name),
            "rating" => filters.SortDescending ? hotelsQuery.OrderByDescending(h => h.Rating) : hotelsQuery.OrderBy(h => h.Rating),
            _ => hotelsQuery.OrderBy(h => h.Name)
        };

        var pagedHotels = await hotelsQuery
            .ProjectTo<GetHotelSlimDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(paginationParameters);

        var result = new GetCountryHotelsDto
        {
            Id = countryId,
            Name = countryName,
            Hotels = pagedHotels
        };

        return Result<GetCountryHotelsDto>.Success(result);
    }
    public async Task<Result> PatchCountryAsync(int id, JsonPatchDocument<UpdateCountryDto> patchDoc)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country is null)
        {
            return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."));
        }

        var countryDto = _mapper.Map<UpdateCountryDto>(country);
        patchDoc.ApplyTo(countryDto);

        if (countryDto.Id != id)
        {
            return Result.BadRequest(new Error(ErrorCodes.Validation, "Cannot modify the Id field."));
        }

        var normalizedName = countryDto.Name.ToLower().Trim();
        var duplicateExists = await _context.Countries
                .AnyAsync(c => c.Name.ToLower().Trim() == normalizedName
                    && c.Id != id);

        if (duplicateExists)
        {
            return Result.Failure(new Error(ErrorCodes.Conflict,
                $"Country with name '{countryDto.Name}' already exists."));
        }

        _mapper.Map(countryDto, country);
        _context.Entry(country).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Result.Success();
    }
}