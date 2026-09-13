using HotelListing.Api.Common.Results;
using HotelListing.Api.DTOs.Auth;

namespace HotelListing.Api.Contracts;

public interface IUsersService
{
    Task<Result<string>> LoginAsync(LoginUserDto loginDto);
    Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
    string UserId { get; }
}
