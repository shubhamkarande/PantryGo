using PantryGo.ViewModels;

namespace PantryGo.Views;

public partial class CheckoutPage : ContentPage
{
    private readonly CheckoutViewModel _viewModel;
    
    public CheckoutPage(CheckoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAddressesCommand.ExecuteAsync(null);
    }
}
