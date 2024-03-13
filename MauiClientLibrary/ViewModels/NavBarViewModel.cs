
namespace MauiClientLibrary.ViewModels;

/// <summary>
/// Navigation bar view model provides the navigation bar
/// </summary>
public partial class NavBarViewModel : BaseViewModel
{
    /// <summary>
    /// Navigate to the current user page
    /// </summary>
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
