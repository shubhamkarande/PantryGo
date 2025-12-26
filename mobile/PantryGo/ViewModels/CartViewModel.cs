using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Models;
using PantryGo.Services;

namespace PantryGo.ViewModels;

public partial class CartViewModel : BaseViewModel
{
    private readonly ICartService _cartService;
    private readonly IAuthService _authService;
    
    [ObservableProperty]
    private ObservableCollection<CartItem> _items;
    
    [ObservableProperty]
    private decimal _totalPrice;
    
    [ObservableProperty]
    private int _itemCount;
    
    [ObservableProperty]
    private bool _isEmpty;
    
    public CartViewModel(ICartService cartService, IAuthService authService)
    {
        _cartService = cartService;
        _authService = authService;
        _items = _cartService.Items;
        Title = "Cart";
        
        _cartService.CartChanged += UpdateCartSummary;
        UpdateCartSummary();
    }
    
    private void UpdateCartSummary()
    {
        TotalPrice = _cartService.TotalPrice;
        ItemCount = _cartService.ItemCount;
        IsEmpty = ItemCount == 0;
    }
    
    [RelayCommand]
    private void IncreaseQuantity(CartItem item)
    {
        if (item.Quantity < item.Stock)
        {
            _cartService.UpdateQuantity(item.ProductId, item.Quantity + 1);
        }
    }
    
    [RelayCommand]
    private void DecreaseQuantity(CartItem item)
    {
        _cartService.UpdateQuantity(item.ProductId, item.Quantity - 1);
    }
    
    [RelayCommand]
    private void RemoveItem(CartItem item)
    {
        _cartService.RemoveItem(item.ProductId);
    }
    
    [RelayCommand]
    private void ClearCart()
    {
        _cartService.ClearCart();
    }
    
    [RelayCommand]
    private async Task CheckoutAsync()
    {
        if (IsEmpty)
        {
            await Shell.Current.DisplayAlert("Empty Cart", "Add items to your cart first", "OK");
            return;
        }
        
        if (!_authService.IsLoggedIn)
        {
            await Shell.Current.GoToAsync("//login");
            return;
        }
        
        await Shell.Current.GoToAsync("checkout");
    }
    
    [RelayCommand]
    private async Task ContinueShoppingAsync()
    {
        await Shell.Current.GoToAsync("//home");
    }
}
