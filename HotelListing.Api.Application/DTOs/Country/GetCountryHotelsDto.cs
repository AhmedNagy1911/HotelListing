using HotelListing.Api.Application.DTOs.Hotel;
using HotelListing.Api.Common.Models.Paging;

namespace HotelListing.Api.Application.DTOs.Country;

public class GetCountryHotelsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PagedResult<GetHotelSlimDto> Hotels { get; set; } = new();
}