namespace HotelListing.Api.Common.Models.Filtering;

public abstract class BaseFilterParameters
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
