using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Models;
using PantryGo.Services;

namespace PantryGo.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly ICartService _cartService;
    
    [ObservableProperty]
    private ObservableCollection<Product> _products = new();
    
    [ObservableProperty]
    private ObservableCollection<string> _categories = new();
    
    [ObservableProperty]
    private string? _selectedCategory;
    
    [ObservableProperty]
    private string _searchQuery = string.Empty;
    
    [ObservableProperty]
    private int _cartItemCount;
    
    public HomeViewModel(IProductService productService, ICartService cartService)
    {
        _productService = productService;
        _cartService = cartService;
        Title = "PantryGo";
        
        _cartService.CartChanged += OnCartChanged;
        CartItemCount = _cartService.ItemCount;
    }
    
    private void OnCartChanged()
    {
        CartItemCount = _cartService.ItemCount;
    }
    
    [RelayCommand]
    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            // Load categories
            var categories = await _productService.GetCategoriesAsync();
            Categories.Clear();
            Categories.Add("All");
            foreach (var cat in categories)
            {
                Categories.Add(cat);
            }
            
            // Load products
            await LoadProductsAsync();
        });
    }
    
    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        await ExecuteAsync(async () =>
        {
            var category = SelectedCategory == "All" ? null : SelectedCategory;
            var search = string.IsNullOrWhiteSpace(SearchQuery) ? null : SearchQuery.Trim();
            
            var products = await _productService.GetProductsAsync(category, search);
            
            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(product);
            }
        });
    }
    
    [RelayCommand]
    private async Task SelectCategoryAsync(string category)
    {
        SelectedCategory = category;
        await LoadProductsAsync();
    }
    
    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadProductsAsync();
    }
    
    [RelayCommand]
    private async Task ViewProductAsync(Product product)
    {
        await Shell.Current.GoToAsync($"product?id={product.Id}");
    }
    
    [RelayCommand]
    private void AddToCart(Product product)
    {
        if (product.Stock > 0)
        {
            _cartService.AddItem(product);
        }
    }
    
    [RelayCommand]
    private async Task GoToCartAsync()
    {
        await Shell.Current.GoToAsync("//cart");
    }
}
