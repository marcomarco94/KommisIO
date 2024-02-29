
namespace MauiClientLibrary.ViewModels;

public partial class MainMenuViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    public MainMenuViewModel(IMainMenuStorage mainMenuStorage, ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        _mainMenu =mainMenuStorage.GetMainMenu();
    }
        
    [ObservableProperty] 
    ObservableCollection<MenuItemModel> _mainMenu;
    
    [RelayCommand]
    private async Task ShowPage(string route)
    {
        await Shell.Current.GoToAsync(route, true);
    }
    
}