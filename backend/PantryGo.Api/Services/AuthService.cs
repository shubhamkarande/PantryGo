using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PantryGo.Api.Data;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Models.Entities;
using PantryGo.Api.Models.Enums;

namespace PantryGo.Api.Services;

public interface IAuthService
{
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
    Task<UserDto?> GetUserByIdAsync(Guid userId);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private static readonly Dictionary<string, (Guid UserId, DateTime Expiry)> _refreshTokens = new();
    
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    
    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        // Check if email exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email.ToLower()))
        {
            return null;
        }
        
        var user = new User
        {
            Email = request.Email.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Name = request.Name,
            Phone = request.Phone,
            Role = UserRole.Customer
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return GenerateAuthResponse(user);
    }
    
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }
        
        return GenerateAuthResponse(user);
    }
    
    public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
    {
        if (!_refreshTokens.TryGetValue(refreshToken, out var tokenData))
        {
            return null;
        }
        
        if (tokenData.Expiry < DateTime.UtcNow)
        {
            _refreshTokens.Remove(refreshToken);
            return null;
        }
        
        var user = await _context.Users.FindAsync(tokenData.UserId);
        if (user == null)
        {
            return null;
        }
        
        _refreshTokens.Remove(refreshToken);
        return GenerateAuthResponse(user);
    }
    
    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : MapToDto(user);
    }
    
    private AuthResponse GenerateAuthResponse(User user)
    {
        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();
        
        _refreshTokens[refreshToken] = (user.Id, DateTime.UtcNow.AddDays(7));
        
        return new AuthResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            User = MapToDto(user)
        };
    }
    
    private string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["Jwt:Secret"] ?? "PantryGoSecretKey12345678901234567890";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "PantryGo",
            audience: _configuration["Jwt:Audience"] ?? "PantryGoApp",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
    
    private static UserDto MapToDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Name = user.Name,
        Phone = user.Phone,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };
}
