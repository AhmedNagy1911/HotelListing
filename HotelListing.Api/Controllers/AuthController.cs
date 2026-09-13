using HotelListing.Api.Application.Contracts;
using HotelListing.Api.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IUsersService usersService) : BaseApiController
{
    private readonly IUsersService _usersService = usersService;

    [HttpPost("register")]
    public async Task<ActionResult<RegisteredUserDto>> Register([FromBody] RegisterUserDto registerUserDto)
    {
        var result = await _usersService.RegisterUserAsync(registerUserDto);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login([FromBody] LoginUserDto loginDto)
    {
        var result = await _usersService.LoginAsync(loginDto);
        return ToActionResult(result);
    }
}
