namespace PantryGo.Views;

public partial class AddressesPage : ContentPage
{
    public AddressesPage()
    {
        InitializeComponent();
    }
    
    private async void OnGoBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
