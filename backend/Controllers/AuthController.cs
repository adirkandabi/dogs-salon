using DogsSalon.DTOs;
using DogsSalon.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogsSalon.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var user = await _authService.Register(dto.Username, dto.Password, dto.FirstName);

        if (user == null)
            return BadRequest(new { message = "Username already exists" });

        return Ok(new { message = "Registration successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _authService.Login(dto.Username, dto.Password);

        if (token == null)
            return Unauthorized(new { message = "Invalid username or password" });

        
        return Ok(new { token });
    }
}