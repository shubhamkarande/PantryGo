using Newtonsoft.Json;
using PantryGo.Helpers;
using PantryGo.Models;

namespace PantryGo.Services;

public interface IAuthService
{
    bool IsLoggedIn { get; }
    User? CurrentUser { get; }
    Task<bool> LoginAsync(string email, string password);
    Task<bool> RegisterAsync(string email, string password, string name, string? phone);
    Task<bool> TryAutoLoginAsync();
    Task LogoutAsync();
    Task<User?> GetCurrentUserAsync();
}

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;
    private User? _currentUser;
    private string? _token;
    private string? _refreshToken;
    
    public bool IsLoggedIn => !string.IsNullOrEmpty(_token);
    public User? CurrentUser => _currentUser;
    
    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var request = new LoginRequest { Email = email, Password = password };
            var response = await _apiService.PostAsync<AuthResponse>("/auth/login", request);
            
            if (response != null)
            {
                await SaveAuthDataAsync(response);
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> RegisterAsync(string email, string password, string name, string? phone)
    {
        try
        {
            var request = new RegisterRequest
            {
                Email = email,
                Password = password,
                Name = name,
                Phone = phone
            };
            
            var response = await _apiService.PostAsync<AuthResponse>("/auth/register", request);
            
            if (response != null)
            {
                await SaveAuthDataAsync(response);
                return true;
            }
            
            return false;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<bool> TryAutoLoginAsync()
    {
        try
        {
            var token = await SecureStorage.GetAsync(Constants.TokenKey);
            var refreshToken = await SecureStorage.GetAsync(Constants.RefreshTokenKey);
            var userJson = await SecureStorage.GetAsync(Constants.UserKey);
            
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userJson))
            {
                return false;
            }
            
            _token = token;
            _refreshToken = refreshToken;
            _currentUser = JsonConvert.DeserializeObject<User>(userJson);
            _apiService.SetAuthToken(_token);
            
            // Verify token is still valid
            var user = await GetCurrentUserAsync();
            return user != null;
        }
        catch
        {
            await LogoutAsync();
            return false;
        }
    }
    
    public async Task LogoutAsync()
    {
        _token = null;
        _refreshToken = null;
        _currentUser = null;
        _apiService.SetAuthToken(null);
        
        try
        {
            SecureStorage.Remove(Constants.TokenKey);
            SecureStorage.Remove(Constants.RefreshTokenKey);
            SecureStorage.Remove(Constants.UserKey);
        }
        catch
        {
            // Ignore storage errors on logout
        }
        
        await Task.CompletedTask;
    }
    
    public async Task<User?> GetCurrentUserAsync()
    {
        try
        {
            var user = await _apiService.GetAsync<User>("/auth/me");
            if (user != null)
            {
                _currentUser = user;
            }
            return user;
        }
        catch
        {
            return null;
        }
    }
    
    private async Task SaveAuthDataAsync(AuthResponse response)
    {
        _token = response.Token;
        _refreshToken = response.RefreshToken;
        _currentUser = response.User;
        
        _apiService.SetAuthToken(_token);
        
        await SecureStorage.SetAsync(Constants.TokenKey, _token);
        await SecureStorage.SetAsync(Constants.RefreshTokenKey, _refreshToken);
        await SecureStorage.SetAsync(Constants.UserKey, JsonConvert.SerializeObject(_currentUser));
    }
}
