
namespace MauiClientLibrary.ViewModels;

public partial class CurrentUserViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;

    public CurrentUserViewModel(ILocalizationService localizationService, IKommissIOAPI kommissIoApi)
    {
        _localizationService = localizationService;
        CurrentUser = kommissIoApi.CurrentEmployee!;
    }
    
    [ObservableProperty] 
    private Employee _currentUser;
}
