
namespace MauiClientLibrary.ViewModels;

/// <summary>
/// MainMenuViewModel provides the main menu
/// </summary>
public partial class MainMenuViewModel : BaseViewModel
{
    /// <summary>
    /// Constructor for the MainMenuViewModel sets up the main menu
    /// </summary>
    /// <param name="mainMenuService"></param>
    public MainMenuViewModel(IMainMenuService mainMenuService)
    {
        _mainMenu =mainMenuService.GetMainMenu();
    }
     
    /// <summary>
    /// Property for the main menu
    /// </summary>
    [ObservableProperty] 
    ObservableCollection<MenuItemModel> _mainMenu;
    
    /// <summary>
    /// Navigates to the selected page
    /// </summary>
    /// <param name="route"></param>
    [RelayCommand]
    private async Task ShowPage(string route)
    {
        await Shell.Current.GoToAsync(route, true);
    }
    
}