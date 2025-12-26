using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Models;
using PantryGo.Services;

namespace PantryGo.ViewModels;

public partial class CheckoutViewModel : BaseViewModel
{
    private readonly ICartService _cartService;
    private readonly IAddressService _addressService;
    private readonly IOrderService _orderService;
    
    [ObservableProperty]
    private ObservableCollection<Address> _addresses = new();
    
    [ObservableProperty]
    private Address? _selectedAddress;
    
    [ObservableProperty]
    private decimal _subtotal;
    
    [ObservableProperty]
    private decimal _deliveryFee = 30.00m;
    
    [ObservableProperty]
    private decimal _total;
    
    [ObservableProperty]
    private bool _isAddressFormVisible;
    
    // New address form
    [ObservableProperty]
    private string _newAddressLabel = "Home";
    
    [ObservableProperty]
    private string _newAddressLine = string.Empty;
    
    [ObservableProperty]
    private string _newCity = string.Empty;
    
    [ObservableProperty]
    private string _newPincode = string.Empty;
    
    public CheckoutViewModel(
        ICartService cartService, 
        IAddressService addressService,
        IOrderService orderService)
    {
        _cartService = cartService;
        _addressService = addressService;
        _orderService = orderService;
        Title = "Checkout";
        
        Subtotal = _cartService.TotalPrice;
        Total = Subtotal + DeliveryFee;
    }
    
    [RelayCommand]
    private async Task LoadAddressesAsync()
    {
        await ExecuteAsync(async () =>
        {
            var addresses = await _addressService.GetAddressesAsync();
            
            Addresses.Clear();
            foreach (var address in addresses)
            {
                Addresses.Add(address);
            }
            
            SelectedAddress = addresses.FirstOrDefault(a => a.IsDefault) ?? addresses.FirstOrDefault();
        });
    }
    
    [RelayCommand]
    private void ShowAddressForm()
    {
        IsAddressFormVisible = true;
    }
    
    [RelayCommand]
    private void HideAddressForm()
    {
        IsAddressFormVisible = false;
        ClearAddressForm();
    }
    
    [RelayCommand]
    private async Task SaveAddressAsync()
    {
        if (string.IsNullOrWhiteSpace(NewAddressLine) || 
            string.IsNullOrWhiteSpace(NewCity) ||
            string.IsNullOrWhiteSpace(NewPincode))
        {
            SetError("Please fill in all address fields");
            return;
        }
        
        await ExecuteAsync(async () =>
        {
            var request = new CreateAddressRequest
            {
                Label = NewAddressLabel,
                AddressLine = NewAddressLine,
                City = NewCity,
                Pincode = NewPincode,
                IsDefault = Addresses.Count == 0
            };
            
            var newAddress = await _addressService.CreateAddressAsync(request);
            
            if (newAddress != null)
            {
                Addresses.Add(newAddress);
                SelectedAddress = newAddress;
                HideAddressForm();
            }
        });
    }
    
    [RelayCommand]
    private async Task PlaceOrderAsync()
    {
        if (SelectedAddress == null)
        {
            await Shell.Current.DisplayAlert("No Address", "Please select or add a delivery address", "OK");
            return;
        }
        
        await ExecuteAsync(async () =>
        {
            var orderItems = _cartService.GetOrderItems();
            var order = await _orderService.CreateOrderAsync(SelectedAddress.Id, orderItems);
            
            if (order != null)
            {
                _cartService.ClearCart();
                
                await Shell.Current.DisplayAlert(
                    "Order Placed! 🎉", 
                    $"Your order #{order.Id.ToString()[..8].ToUpper()} has been placed successfully!", 
                    "OK"
                );
                
                await Shell.Current.GoToAsync($"//orders/detail?id={order.Id}");
            }
        });
    }
    
    private void ClearAddressForm()
    {
        NewAddressLabel = "Home";
        NewAddressLine = string.Empty;
        NewCity = string.Empty;
        NewPincode = string.Empty;
    }
}
