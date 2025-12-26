using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Models;
using PantryGo.Services;

namespace PantryGo.ViewModels;

[QueryProperty(nameof(ProductId), "id")]
public partial class ProductDetailViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    
    [ObservableProperty]
    private Guid _productId;
    
    [ObservableProperty]
    private Product? _product;
    
    [ObservableProperty]
    private int _quantity = 1;
    
    [ObservableProperty]
    private bool _isInStock;
    
    public ProductDetailViewModel(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
    }
    
    partial void OnProductIdChanged(Guid value)
    {
        LoadProductCommand.Execute(null);
    }
    
    [RelayCommand]
    private async Task LoadProductAsync()
    {
        if (ProductId == Guid.Empty) return;
        
        await ExecuteAsync(async () =>
        {
            Product = await _productService.GetProductByIdAsync(ProductId);
            if (Product != null)
            {
                Title = Product.Name;
                IsInStock = Product.Stock > 0;
            }
        });
    }
    
    [RelayCommand]
    private void IncreaseQuantity()
    {
        if (Product != null && Quantity < Product.Stock)
        {
            Quantity++;
        }
    }
    
    [RelayCommand]
    private void DecreaseQuantity()
    {
        if (Quantity > 1)
        {
            Quantity--;
        }
    }
    
    [RelayCommand]
    private async Task AddToCartAsync()
    {
        if (Product == null || !IsInStock) return;
        
        _cartService.AddItem(Product, Quantity);
        
        await Shell.Current.DisplayAlert(
            "Added to Cart", 
            $"{Quantity} x {Product.Name} added to cart", 
            "OK"
        );
        
        await Shell.Current.GoToAsync("..");
    }
    
    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
