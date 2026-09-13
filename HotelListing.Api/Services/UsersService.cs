using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Models;
using HotelListing.Api.Common.Results;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.Api.Services;

public class UsersService(UserManager<ApplicationUser> userManager,
    IOptions<JwtSettings> jwtOptions,
    IHttpContextAccessor httpContextAccessor,
    HotelListingDbContext hotelListingDbContext) : IUsersService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtSettings _jwtOptions = jwtOptions.Value;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var user = new ApplicationUser
        {
            Email = registerUserDto.Email,
            UserName = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName
        };
        var result = await _userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
            return Result<RegisteredUserDto>.BadRequest(errors);
        }

        await _userManager.AddToRoleAsync(user, registerUserDto.Role);

        // If Hotel Admin, add to HotelAdmins table
        if (registerUserDto.Role  == RoleNames.HotelAdmin)
        {
            var hotelAdmin = hotelListingDbContext.HotelAdmins.Add(
                new HotelAdmin
                {
                    UserId = user.Id,
                    HotelId = registerUserDto.AssociatedHotelId.GetValueOrDefault()
                });
            await hotelListingDbContext.SaveChangesAsync();
        }

        var registeredUserDto = new RegisteredUserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = registerUserDto.Role
        };
        return Result<RegisteredUserDto>.Success(registeredUserDto);
    }

    public async Task<Result<string>> LoginAsync(LoginUserDto loginDto)
    {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid email or password."));

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordValid)
            return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid email or password."));

        var token = await GenerateToken(user);

        return Result<string>.Success(token);
    }

    private async Task<string> GenerateToken(ApplicationUser user)
    {
        // Set basic user claims
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Sub, user.Id),
            new (JwtRegisteredClaimNames.Email, user.Email!),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Name, user.FullName)
        };

        // Set user role claims
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

        claims = claims.Union(roleClaims).ToList();

        // Set JWT Key credentials
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create an encoded token
        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.DurationInMinutes),
            signingCredentials: credentials
            );

        // Return token value
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string UserId => _httpContextAccessor?
           .HttpContext?
           .User?
           .FindFirst(JwtRegisteredClaimNames.Sub)?.Value
       ?? _httpContextAccessor?
           .HttpContext?
           .User?
           .FindFirst(ClaimTypes.NameIdentifier)?.Value
       ?? string.Empty;
}
