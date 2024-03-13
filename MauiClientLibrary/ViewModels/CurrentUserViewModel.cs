
namespace MauiClientLibrary.ViewModels;

/// <summary>
/// The CurrentUserViewModel provides the current user
/// </summary>
public partial class CurrentUserViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoApi;

    /// <summary>
    /// Set up the CurrentUser and the regarding roles and services
    /// </summary>
    /// <param name="localizationService"></param>
    /// <param name="kommissIoApi"></param>
    public CurrentUserViewModel(ILocalizationService localizationService, IKommissIOAPI kommissIoApi)
    {
        _localizationService = localizationService;
        _kommissIoApi = kommissIoApi;
        CurrentUser = _kommissIoApi.CurrentEmployee!;
        IsAdmin = CurrentUser.Role.HasFlag(Role.Administrator);
        GetUserRoles();
    }
    
    /// <summary>
    /// Gets the roles of the current user
    /// </summary>
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
    
    /// <summary>
    /// Property for the current user roles
    /// </summary>
    [ObservableProperty]
    ObservableCollection<String> _currentUserRoles = new ();

    /// <summary>
    /// Property for the current user
    /// </summary>
    [ObservableProperty]
    private Employee _currentUser;

    /// <summary>
    /// True if the user is an admin
    /// </summary>
    [ObservableProperty]
    private bool _isAdmin;

    /// <summary>
    /// Command to reset the data to default
    /// </summary>
    [RelayCommand]
    private async Task ResetDataAsync()
    {
        if (IsAdmin)
        {
            await _kommissIoApi.ResetToDefaultAsync();
        }
    }
}
