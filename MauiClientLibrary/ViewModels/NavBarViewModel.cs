
namespace MauiClientLibrary.ViewModels;

public partial class NavBarViewModel : BaseViewModel
{
    [RelayCommand]
    private async Task  ShowUserPage()
    {
        var currentPage = Shell.Current.CurrentPage.GetType().Name;
        var userPage = "CurrentUserPage";
        if (currentPage == userPage)
        {
            return;
        }
        await Shell.Current.GoToAsync(userPage, true);
    }
    
}
