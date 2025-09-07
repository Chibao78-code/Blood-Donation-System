using System.Security.Cryptography;
using System.Text;
using BloodDonation.Application.DTOs;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using BloodDonation.Domain.Enums;
using BloodDonation.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BloodDonation.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<(bool Success, string Message, UserDto? User)> LoginAsync(LoginDto loginDto)
    {
        var user = await _unitOfWork.Users
            .Query()
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null)
            return (false, "Tài khoản không tồn tại.", null);

        if (!VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt ?? ""))
            return (false, "Mật khẩu không đúng.", null);

        if (!user.IsActive)
            return (false, "Tài khoản đã bị khóa.", null);

        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt
        };

        return (true, "Đăng nhập thành công.", userDto);
    }

    public async Task<(bool Success, string Message)> RegisterAsync(RegisterDto registerDto)
    {
        if (await UserExistsAsync(registerDto.Username))
            return (false, "Tên đăng nhập đã tồn tại.");

        if (await EmailExistsAsync(registerDto.Email))
            return (false, "Email đã được sử dụng.");

        if (registerDto.Password != registerDto.ConfirmPassword)
            return (false, "Mật khẩu xác nhận không khớp.");

        var (passwordHash, passwordSalt) = HashPassword(registerDto.Password);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = UserRole.Donor,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var donor = new Donor
        {
            UserId = user.Id,
            FullName = registerDto.FullName,
            PhoneNumber = registerDto.PhoneNumber,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Donors.AddAsync(donor);
        await _unitOfWork.SaveChangesAsync();

        return (true, "Đăng ký thành công.");
    }

    public async Task<bool> UserExistsAsync(string username)
    {
        return await _unitOfWork.Users
            .Query()
            .AnyAsync(u => u.Username.ToLower() == username.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _unitOfWork.Users
            .Query()
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }

    private (string hash, string salt) HashPassword(string password)
    {
        using var hmac = new HMACSHA512();
        var salt = Convert.ToBase64String(hmac.Key);
        var hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return (hash, salt);
    }

    private bool VerifyPassword(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        using var hmac = new HMACSHA512(saltBytes);
        var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        return computedHash == hash;
    }
}
