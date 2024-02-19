
namespace MauiClientLibrary.ViewModels;

public partial class CurrentUserViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoApi;
    
    public CurrentUserViewModel(ILocalizationService localizationService, IKommissIOAPI kommissIoApi)
    {
        _localizationService = localizationService;
        _kommissIoApi = kommissIoApi;
        CurrentUser = _kommissIoApi.CurrentEmployee!;
    }
    
    [ObservableProperty] 
    private Employee _currentUser;
    
    [RelayCommand]
    private async Task  ShowUserPage()
    {
        CurrentUser = _kommissIoApi.CurrentEmployee!;
        IsLoggedIn = CurrentUser != null;
        if (IsLoggedIn) {
            await Shell.Current.GoToAsync("UserPage", true);
        }
    }
}
