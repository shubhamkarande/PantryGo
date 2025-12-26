using PantryGo.Models;

namespace PantryGo.Services;

public interface IOrderService
{
    Task<Order?> CreateOrderAsync(Guid addressId, List<CreateOrderItemRequest> items);
    Task<List<Order>> GetOrdersAsync(int page = 1, int pageSize = 20);
    Task<Order?> GetOrderByIdAsync(Guid id);
}

public class OrderService : IOrderService
{
    private readonly IApiService _apiService;
    
    public OrderService(IApiService apiService)
    {
        _apiService = apiService;
    }
    
    public async Task<Order?> CreateOrderAsync(Guid addressId, List<CreateOrderItemRequest> items)
    {
        var request = new CreateOrderRequest
        {
            AddressId = addressId,
            Items = items
        };
        
        return await _apiService.PostAsync<Order>("orders", request);
    }
    
    public async Task<List<Order>> GetOrdersAsync(int page = 1, int pageSize = 20)
    {
        var response = await _apiService.GetAsync<PagedResponse<Order>>($"orders?page={page}&pageSize={pageSize}");
        return response?.Items ?? new List<Order>();
    }
    
    public async Task<Order?> GetOrderByIdAsync(Guid id)
    {
        return await _apiService.GetAsync<Order>($"orders/{id}");
    }
}
