namespace DogsSalon.DTOs;

public record RegisterDto(string Username, string Password, string FirstName);
public record LoginDto(string Username, string Password);
public record AuthResponseDto(string Token, string Username, string FirstName);