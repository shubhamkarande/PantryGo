using PantryGo.ViewModels;

namespace PantryGo.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;
    
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
