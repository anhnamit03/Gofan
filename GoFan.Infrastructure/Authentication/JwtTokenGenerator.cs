using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoFan.Application.Interfaces.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GoFan.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt) GenerateToken(int userId, string role, string identifier, string? name)
    {
        var secret = _configuration["JwtSettings:Secret"] ?? "GoFan_Ultra_Secure_Secret_Key_For_JWT_Authentication_2026_Minimum_32_Bytes!";
        var issuer = _configuration["JwtSettings:Issuer"] ?? "GoFan";
        var audience = _configuration["JwtSettings:Audience"] ?? "GoFanApp";
        var expiryMinutesString = _configuration["JwtSettings:ExpiryMinutes"];
        var expiryMinutes = int.TryParse(expiryMinutesString, out var m) ? m : 1440; // Default 24h

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Role, role),
            new(ClaimTypes.Name, identifier),
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("role", role)
        };

        if (!string.IsNullOrWhiteSpace(name))
        {
            claims.Add(new Claim("fullName", name));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return (tokenHandler.WriteToken(token), expiresAt);
    }
}
