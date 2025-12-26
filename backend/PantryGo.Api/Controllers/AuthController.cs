using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PantryGo.Api.Models.DTOs;
using PantryGo.Api.Services;

namespace PantryGo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        
        if (result == null)
        {
            return BadRequest(ApiResponse<object>.Fail("Email already registered"));
        }
        
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Registration successful"));
    }
    
    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        
        if (result == null)
        {
            return Unauthorized(ApiResponse<object>.Fail("Invalid email or password"));
        }
        
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful"));
    }
    
    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request.RefreshToken);
        
        if (result == null)
        {
            return Unauthorized(ApiResponse<object>.Fail("Invalid or expired refresh token"));
        }
        
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Token refreshed"));
    }
    
    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.GetUserId();
        var user = await _authService.GetUserByIdAsync(userId);
        
        if (user == null)
        {
            return NotFound(ApiResponse<object>.Fail("User not found"));
        }
        
        return Ok(ApiResponse<UserDto>.Ok(user));
    }
}
