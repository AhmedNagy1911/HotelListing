using HotelListing.Api.Application.DTOs.Auth;
using HotelListing.Api.Common.Results;

namespace HotelListing.Api.Application.Contracts;

public interface IUsersService
{
    Task<Result<string>> LoginAsync(LoginUserDto loginDto);
    Task<Result<RegisteredUserDto>> RegisterUserAsync(RegisterUserDto registerUserDto);
    string UserId { get; }
}
