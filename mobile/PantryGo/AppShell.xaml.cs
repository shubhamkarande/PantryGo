using PantryGo.Views;

namespace PantryGo;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        
        // Register routes for navigation
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("product", typeof(ProductDetailPage));
        Routing.RegisterRoute("checkout", typeof(CheckoutPage));
        Routing.RegisterRoute("detail", typeof(OrderDetailPage));
        Routing.RegisterRoute("addresses", typeof(AddressesPage));
    }
}
