
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
        IsAdmin = CurrentUser?.Role == Role.Administrator;
    }

    [ObservableProperty]
    private Employee _currentUser;

    [ObservableProperty]
    private bool _isAdmin;

    [RelayCommand]
    private async Task ResetDataAsync()
    {
        if (IsAdmin)
        {
            await _kommissIoApi.ResetToDefaultAsync();
        }
    }
}
