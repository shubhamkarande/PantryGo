using PantryGo.Models;

namespace PantryGo.Services;

public interface IAddressService
{
    Task<List<Address>> GetAddressesAsync();
    Task<Address?> CreateAddressAsync(CreateAddressRequest request);
    Task<Address?> SetDefaultAddressAsync(Guid addressId);
    Task<bool> DeleteAddressAsync(Guid addressId);
}

public class AddressService : IAddressService
{
    private readonly IApiService _apiService;
    
    public AddressService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    public async Task<List<Address>> GetAddressesAsync()
    {
        var addresses = await _apiService.GetAsync<List<Address>>("/addresses");
        return addresses ?? new List<Address>();
    }
    
    public async Task<Address?> CreateAddressAsync(CreateAddressRequest request)
    {
        return await _apiService.PostAsync<Address>("/addresses", request);
    }
    
    public async Task<Address?> SetDefaultAddressAsync(Guid addressId)
    {
        return await _apiService.PutAsync<Address>($"/addresses/{addressId}/default");
    }
    
    public async Task<bool> DeleteAddressAsync(Guid addressId)
    {
        return await _apiService.DeleteAsync($"/addresses/{addressId}");
    }
}
