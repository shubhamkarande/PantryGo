using PantryGo.Models;

namespace PantryGo.Services;

public interface IProductService
{
    Task<List<Product>> GetProductsAsync(string? category = null, string? search = null);
    Task<Product?> GetProductByIdAsync(Guid id);
    Task<List<string>> GetCategoriesAsync();
}

public class ProductService : IProductService
{
    private readonly IApiService _apiService;
    
    public ProductService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    public async Task<List<Product>> GetProductsAsync(string? category = null, string? search = null)
    {
        var queryParams = new List<string>();
        
        if (!string.IsNullOrEmpty(category))
            queryParams.Add($"category={Uri.EscapeDataString(category)}");
            
        if (!string.IsNullOrEmpty(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");
        
        queryParams.Add("pageSize=100"); // Get all products
        
        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var response = await _apiService.GetAsync<PagedResponse<Product>>($"/products{query}");
        
        return response?.Items ?? new List<Product>();
    }
    
    public async Task<Product?> GetProductByIdAsync(Guid id)
    {
        return await _apiService.GetAsync<Product>($"/products/{id}");
    }
    
    public async Task<List<string>> GetCategoriesAsync()
    {
        var categories = await _apiService.GetAsync<List<string>>("/products/categories");
        return categories ?? new List<string>();
    }
}
