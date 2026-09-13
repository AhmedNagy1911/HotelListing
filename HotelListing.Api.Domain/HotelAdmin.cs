namespace HotelListing.Api.Domain;

public class HotelAdmin
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int HotelId { get; set; }

    public Hotel? Hotel { get; set; } 
    public ApplicationUser? User { get; set; }
}
