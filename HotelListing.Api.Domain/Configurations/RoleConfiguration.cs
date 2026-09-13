using HotelListing.Api.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Api.Domain.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "c78e8f15-6a6c-4c8a-b5d1-98394b071953",
                Name = RoleNames.Administrator,
                NormalizedName = RoleNames.Administrator.ToUpper(),
                ConcurrencyStamp = "a1c4f8e2-7d31-4b91-9c25-123456789001"
            },
            new IdentityRole
            {
                Id = "36aac992-72ff-4527-9008-52e7c145ca39",
                Name = RoleNames.User,
                NormalizedName = RoleNames.User.ToUpper(),
                ConcurrencyStamp = "b2d5e9f3-8e42-4ca2-ad36-123456789002"
            },
            new IdentityRole
            {
                Id = "36aac992-4c8a-4527-9008-98394b071953",
                Name = RoleNames.HotelAdmin,
                NormalizedName = RoleNames.HotelAdmin.ToUpper(),
                ConcurrencyStamp = "c3e6f0a4-9f53-4db3-bd47-123456789003"
            }
        );
    }
}