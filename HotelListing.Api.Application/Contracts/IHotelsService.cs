using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Common.Models.Paging;
using HotelListing.Api.Common.Results;

namespace HotelListing.Api.Application.Contracts;

public interface IHotelsService
{
    Task<bool> HotelExistsAsync(int id);
    Task<bool> HotelExistsAsync(string name, int countryId);
    Task<Result<PagedResult<GetHotelDto>>> GetHotelsAsync(PaginationParameters paginationParameters);
    Task<Result<GetHotelDto>> GetHotelAsync(int id);
    Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto createDto);
    Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateDto);
    Task<Result> DeleteHotelAsync(int id);
}