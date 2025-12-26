using PantryGo.Services;

namespace PantryGo;

public partial class App : Application
{
    private readonly IAuthService _authService;
    
    public App(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    protected override async void OnStart()
    {
        base.OnStart();
        
        // Try auto-login with stored credentials
        var isLoggedIn = await _authService.TryAutoLoginAsync();
        
        if (isLoggedIn)
        {
            await Shell.Current.GoToAsync("//home");
        }
        else
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}
