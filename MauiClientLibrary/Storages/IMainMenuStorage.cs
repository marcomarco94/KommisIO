namespace MauiClientLibrary.Storages;

public interface IMainMenuStorage
{
    ObservableCollection<MenuItemModel> GetMainMenu();
}