
namespace MauiClientLibrary.ViewModels;

public partial class NavBarViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task  ShowUserPage()
    {
        await Shell.Current.GoToAsync("CurrentUserPage", true);
    }
    
}
