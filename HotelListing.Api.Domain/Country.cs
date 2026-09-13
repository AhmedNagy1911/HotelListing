namespace HotelListing.Api.Domain;

public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string ShortName { get; set; } = string.Empty;
    public List<Hotel> Hotels { get; set; } = [];
}
