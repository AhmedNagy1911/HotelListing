using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Data;

public class HotelListingDbContext(DbContextOptions<HotelListingDbContext> options) 
    :IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
}
