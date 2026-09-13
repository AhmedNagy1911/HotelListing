namespace HotelListing.Api.Domain;

public class Hotel
{
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Address { get; set; } = string.Empty;
    public double Rating { get; set; }
    public decimal PerNightRate { get; set; }

    public int CountryId { get; set; }
    public Country? Country { get; set; } = default!;
    public ICollection<HotelAdmin> Admins { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}
