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

   