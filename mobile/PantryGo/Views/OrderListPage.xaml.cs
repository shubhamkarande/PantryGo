using PantryGo.ViewModels;

namespace PantryGo.Views;

public partial class OrderListPage : ContentPage
{
    private readonly OrderListViewModel _viewModel;
    
    public OrderListPage(OrderListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadOrdersCommand.ExecuteAsync(null);
    }
}
