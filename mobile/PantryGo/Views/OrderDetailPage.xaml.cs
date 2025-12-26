using PantryGo.ViewModels;

namespace PantryGo.Views;

public partial class OrderDetailPage : ContentPage
{
    public OrderDetailPage(OrderDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
