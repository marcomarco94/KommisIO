
namespace MauiClientLibrary.ViewModels;

public partial class MainMenuViewModel : BaseViewModel
{ 
    private readonly IKommissIOAPI _kommissIoApi;
    private readonly IMainMenuStorage _mainMenuStorage;
    private readonly ILocalizationService _localizationService;
    public MainMenuViewModel(IMainMenuStorage mainMenuStorage, ILocalizationService localizationService)
    {
        _mainMenuStorage = mainMenuStorage;
        _localizationService = localizationService;
         _mainMenu =_mainMenuStorage.GetMainMenu();
    }
        
    [ObservableProperty] 
    ObservableCollection<MenuItemModel> _mainMenu;
    
    [RelayCommand]
    private async Task ShowPage(string route)
    {
        await Shell.Current.GoToAsync(route, true);
    }
    
}