namespace GoFan.Application.Interfaces.Authentication;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAt) GenerateToken(int userId, string role, string identifier, string? name);
}
