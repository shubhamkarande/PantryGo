using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Models;
using PantryGo.Services;

namespace PantryGo.ViewModels;

public partial class OrderListViewModel : BaseViewModel
{
    private readonly IOrderService _orderService;
    
    [ObservableProperty]
    private ObservableCollection<Order> _orders = new();
    
    [ObservableProperty]
    private bool _isEmpty;
    
    [ObservableProperty]
    private bool _isRefreshing;
    
    public OrderListViewModel(IOrderService orderService)
    {
        _orderService = orderService;
        Title = "My Orders";
    }
    
    [RelayCommand]
    private async Task LoadOrdersAsync()
    {
        await ExecuteAsync(async () =>
        {
            var orders = await _orderService.GetOrdersAsync();
            
            Orders.Clear();
            foreach (var order in orders)
            {
                Orders.Add(order);
            }
            
            IsEmpty = Orders.Count == 0;
        });
        
        IsRefreshing = false;
    }
    
    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await LoadOrdersAsync();
    }
    
    [RelayCommand]
    private async Task ViewOrderAsync(Order order)
    {
        await Shell.Current.GoToAsync($"detail?id={order.Id}");
    }
}
