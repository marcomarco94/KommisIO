namespace MauiClientLibrary.Services;

/// <summary>
/// MainMenuService provides the items for the main menu
/// </summary>
public interface IMainMenuService
{
    /// <summary>
    /// Function to get the main menu items based on the user role
    /// </summary>
    /// <returns></returns>
    ObservableCollection<MenuItemModel> GetMainMenu();
}