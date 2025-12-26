using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Services;

namespace PantryGo.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Login";
    }
    
    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            SetError("Please enter email and password");
            return;
        }
        
        await ExecuteAsync(async () =>
        {
            var success = await _authService.LoginAsync(Email.Trim(), Password);
            
            if (success)
            {
                await Shell.Current.GoToAsync("//home");
            }
            else
            {
                SetError("Invalid email or password");
            }
        });
    }
    
    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("register");
    }
}
