using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Helpers;
using PantryGo.Models;
using PantryGo.Services;

namespace PantryGo.ViewModels;

[QueryProperty(nameof(OrderId), "id")]
public partial class OrderDetailViewModel : BaseViewModel
{
    private readonly IOrderService _orderService;
    
    [ObservableProperty]
    private Guid _orderId;
    
    [ObservableProperty]
    private Order? _order;
    
    [ObservableProperty]
    private string _statusText = string.Empty;
    
    [ObservableProperty]
    private string _statusIcon = string.Empty;
    
    [ObservableProperty]
    private Color _statusColor = Colors.Gray;
    
    public OrderDetailViewModel(IOrderService orderService)
    {
        _orderService = orderService;
        Title = "Order Details";
    }
    
    partial void OnOrderIdChanged(Guid value)
    {
        LoadOrderCommand.Execute(null);
    }
    
    [RelayCommand]
    private async Task LoadOrderAsync()
    {
        if (OrderId == Guid.Empty) return;
        
        await ExecuteAsync(async () =>
        {
            Order = await _orderService.GetOrderByIdAsync(OrderId);
            
            if (Order != null)
            {
                Title = $"Order #{Order.Id.ToString()[..8].ToUpper()}";
                StatusText = Order.Status.ToString();
                StatusIcon = Constants.GetStatusIcon(StatusText);
                StatusColor = Constants.GetStatusColor(StatusText);
            }
        });
    }
    
    [RelayCommand]
    private async Task RefreshOrderAsync()
    {
        await LoadOrderAsync();
    }
    
    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
