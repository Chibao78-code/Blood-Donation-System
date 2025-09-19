using BloodDonation.Domain.Entities;

namespace BloodDonation.Application.Interfaces;

public interface ITokenService
{

    string GenerateAccessToken(User user);
    

    string GenerateRefreshToken();
    

    Task<bool> ValidateTokenAsync(string token);
    

    int? GetUserIdFromToken(string token);
}