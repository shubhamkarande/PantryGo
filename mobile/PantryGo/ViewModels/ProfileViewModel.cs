using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Models;
using PantryGo.Services;

namespace PantryGo.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    
    [ObservableProperty]
    private User? _user;
    
    [ObservableProperty]
    private bool _isLoggedIn;
    
    public ProfileViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Profile";
    }
    
    [RelayCommand]
    private void LoadProfile()
    {
        User = _authService.CurrentUser;
        IsLoggedIn = _authService.IsLoggedIn;
    }
    
    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Logout", 
            "Are you sure you want to logout?", 
            "Yes", "No"
        );
        
        if (confirm)
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//login");
        }
    }
    
    [RelayCommand]
    private async Task ViewOrdersAsync()
    {
        await Shell.Current.GoToAsync("//orders");
    }
    
    [RelayCommand]
    private async Task ManageAddressesAsync()
    {
        await Shell.Current.GoToAsync("addresses");
    }
}
