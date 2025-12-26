using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PantryGo.Services;

namespace PantryGo.ViewModels;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _confirmPassword = string.Empty;
    
    [ObservableProperty]
    private string _phone = string.Empty;
    
    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Create Account";
    }
    
    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            SetError("Please enter your name");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Email))
        {
            SetError("Please enter your email");
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        {
            SetError("Password must be at least 6 characters");
            return;
        }
        
        if (Password != ConfirmPassword)
        {
            SetError("Passwords do not match");
            return;
        }
        
        await ExecuteAsync(async () =>
        {
            var success = await _authService.RegisterAsync(
                Email.Trim(), 
                Password, 
                Name.Trim(), 
                string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim()
            );
            
            if (success)
            {
                await Shell.Current.GoToAsync("//home");
            }
            else
            {
                SetError("Registration failed. Email may already be in use.");
            }
        });
    }
    
    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
