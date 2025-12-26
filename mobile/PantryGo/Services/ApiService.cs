using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using PantryGo.Helpers;
using PantryGo.Models;

namespace PantryGo.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object? data = null);
    Task<T?> PutAsync<T>(string endpoint, object? data = null);
    Task<bool> DeleteAsync(string endpoint);
    void SetAuthToken(string? token);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private string? _authToken;
    
    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(Constants.ApiBaseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }
    
    public void SetAuthToken(string? token)
    {
        _authToken = token;
        
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
    
    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API GET Error: {ex.Message}");
            throw;
        }
    }
    
    public async Task<T?> PostAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            var content = data != null
                ? new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                : null;
                
            var response = await _httpClient.PostAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API POST Error: {ex.Message}");
            throw;
        }
    }
    
    public async Task<T?> PutAsync<T>(string endpoint, object? data = null)
    {
        try
        {
            var content = data != null
                ? new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json")
                : null;
                
            var response = await _httpClient.PutAsync(endpoint, content);
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API PUT Error: {ex.Message}");
            throw;
        }
    }
    
    public async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API DELETE Error: {ex.Message}");
            throw;
        }
    }
    
    private static async Task<T?> HandleResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var errorResponse = JsonConvert.DeserializeObject<ApiResponse<object>>(content);
                throw new Exception(errorResponse?.Message ?? $"API Error: {response.StatusCode}");
            }
            catch (JsonException)
            {
                throw new Exception($"API Error: {response.StatusCode} - {content}");
            }
        }
        
        // Try to parse as ApiResponse<T> first
        try
        {
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<T>>(content);
            if (apiResponse != null && apiResponse.Success)
            {
                return apiResponse.Data;
            }
        }
        catch (JsonException)
        {
            // Fall through to direct deserialize
        }
        
        return JsonConvert.DeserializeObject<T>(content);
    }
}
