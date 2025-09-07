using BloodDonation.Application.DTOs;

namespace BloodDonation.Application.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginDto loginDto);
    Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto);
    Task<bool> UserExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task LogoutAsync();
}
