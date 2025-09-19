using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BloodDonation.Application.Interfaces;
using BloodDonation.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BloodDonation.Infrastructure.Services;
public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Lấy config từ appsettings, nếu không có thì dùng default
        _secretKey = _configuration["Jwt:SecretKey"] ?? "BloodDonationSystemSecretKeyForJWT2025VeryLongKeyForSecurity";
        _issuer = _configuration["Jwt:Issuer"] ?? "BloodDonationSystem";
        _audience = _configuration["Jwt:Audience"] ?? "BloodDonationUsers";
        _expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"] ?? "60");
    }
     public string GenerateAccessToken(User user)
    {
        try
        {
            // Tạo security key từ secret key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Danh sách claims - thông tin user trong token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("UserId", user.Id.ToString()), // Custom claim cho dễ lấy
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID unique
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Thêm Medical Center ID nếu user là staff của trung tâm
            if (user.MedicalCenterId.HasValue)
            {
                claims.Add(new Claim("MedicalCenterId", user.MedicalCenterId.Value.ToString()));
            }
            // Tạo token với thời hạn
            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
                signingCredentials: credentials
            );

            // Convert token thành string
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            // Log lỗi nếu có vấn đề khi tạo token
            // Trong production nên dùng ILogger
            Console.WriteLine($"Lỗi khi tạo token: {ex.Message}");
            throw new Exception("Không thể tạo access token", ex);
        }
    }

   