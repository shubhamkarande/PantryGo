using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PantryGo.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool _isBusy;
    
    [ObservableProperty]
    private string _title = string.Empty;
    
    [ObservableProperty]
    private string? _errorMessage;
    
    [ObservableProperty]
    private bool _hasError;
    
    public bool IsNotBusy => !IsBusy;
    
    protected void SetError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }
    
    protected void ClearError()
    {
        ErrorMessage = null;
        HasError = false;
    }
    
    protected async Task<bool> ExecuteAsync(Func<Task> action, string? loadingMessage = null)
    {
        if (IsBusy) return false;
        
        try
        {
            IsBusy = true;
            ClearError();
            await action();
            return true;
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    protected async Task<T?> ExecuteAsync<T>(Func<Task<T>> action)
    {
        if (IsBusy) return default;
        
        try
        {
            IsBusy = true;
            ClearError();
            return await action();
        }
        catch (Exception ex)
        {
            SetError(ex.Message);
            return default;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
