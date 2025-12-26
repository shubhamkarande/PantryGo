using PantryGo.ViewModels;

namespace PantryGo.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadProfileCommand.Execute(null);
    }
}
