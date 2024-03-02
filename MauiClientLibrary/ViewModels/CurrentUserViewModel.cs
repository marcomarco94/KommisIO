
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
        IsAdmin = CurrentUser.Role.HasFlag(Role.Administrator);
        GetUserRoles();
    }
    
    private void GetUserRoles()
    {
        CurrentUserRoles.Clear();
        foreach (Role role in Enum.GetValues(typeof(Role)))
        {
            if (CurrentUser.Role.HasFlag(role))
            {
                CurrentUserRoles.Add(role.ToString());
            }
        }
    }
    
    [ObservableProperty]
    ObservableCollection<String> _currentUserRoles = new ();

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
