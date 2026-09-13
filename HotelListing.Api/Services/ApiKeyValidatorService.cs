using HotelListing.Api.Contracts;
using HotelListing.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Api.Services;

public class ApiKeyValidatorService(HotelListingDbContext db) : IApiKeyValidatorService
{
    public async Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) return false;

        var apiKeyEntity = await db.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.Key == apiKey, ct);

        if (apiKeyEntity is null) return false;

        // If there is no expiry date or the expiry date does not exceed today's date.
        return apiKeyEntity.IsActive;
    }
}