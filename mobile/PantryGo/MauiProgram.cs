using Microsoft.Extensions.Logging;
using PantryGo.Services;
using PantryGo.ViewModels;
using PantryGo.Views;

namespace PantryGo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services (Singleton for shared state)
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ICartService, CartService>();
        builder.Services.AddSingleton<IProductService, ProductService>();
        builder.Services.AddSingleton<IOrderService, OrderService>();
        builder.Services.AddSingleton<IAddressService, AddressService>();

        // Register ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<ProductDetailViewModel>();
        builder.Services.AddTransient<CartViewModel>();
        builder.Services.AddTransient<CheckoutViewModel>();
        builder.Services.AddTransient<OrderListViewModel>();
        builder.Services.AddTransient<OrderDetailViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Register Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<ProductDetailPage>();
        builder.Services.AddTransient<CartPage>();
        builder.Services.AddTransient<CheckoutPage>();
        builder.Services.AddTransient<OrderListPage>();
        builder.Services.AddTransient<OrderDetailPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<AddressesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
