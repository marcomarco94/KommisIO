
namespace MauiClientLibrary.ViewModels;

/// <summary>
/// The LoginViewModel provides the login function
/// </summary>
public partial class LoginViewModel : BaseViewModel
{
    private readonly IKommissIOAPI _kommissIoApi;
    private readonly ILocalizationService _localizationService;
    
    /// <summary>
    /// The LoginViewModel constructor. Set up the necessary services
    /// </summary>
    /// <param name="kommissIoApi"></param>
    /// <param name="localizationService"></param>
    public LoginViewModel(IKommissIOAPI kommissIoApi, ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        _kommissIoApi = kommissIoApi;
    }
    
    /// <summary>
    /// Property for the users personnel number
    /// </summary>
    [Required]
    [Range(0, 9999)]
    [ObservableProperty]
    string _personnelNumber = string.Empty;

    /// <summary>
    /// Property for the users password
    /// </summary>
    [Required]
    [MinLength(4)]
    [MaxLength(30)]
    [ObservableProperty]
    string _password = string.Empty;

    /// <summary>
    /// True if the password is valid
    /// </summary>
    [ObservableProperty]
    bool  _isPasswordValid;

    /// <summary>
    /// True if the personnel number is valid
    /// </summary>
    [ObservableProperty]
    bool _isPersonnelNumberValid;
    
    /// <summary>
    /// Checks if the entries are valid
    /// </summary>
    [RelayCommand]
    private void ValidateEntries()
    {
        ValidateAllProperties();
        IsPersonnelNumberValid = !GetErrors(nameof(PersonnelNumber)).Any();
        IsPasswordValid = !GetErrors(nameof(Password)).Any();
    }

    /// <summary>
    /// Logs out the current user
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private Task LogOutAsync()
    {
            PersonnelNumber = string.Empty;
            Password = string.Empty;
            IsPersonnelNumberValid = false;
            IsPasswordValid = false;
            _kommissIoApi.CurrentEmployee = null;
            return Task.CompletedTask;
    }
    
    /// <summary>
    /// Try to login the user with the given credentials
    /// </summary>
    [RelayCommand]
    private async Task LoginAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            var validationResults = GetErrors();
            await DisplayInvalidLoginAlert();
            return;
        }

        bool canParse = short.TryParse(PersonnelNumber, out var personnelNumberParsed);
        if (!canParse)
        {
         await  DisplayInvalidLoginAlert();
         return;
        }
        IsBusy = true;

        try
        {
            await _kommissIoApi.IdentifyAndAuthenticateAysnc(personnelNumberParsed, Password);
            if (_kommissIoApi.CurrentEmployee == null)
            {
                await DisplayInvalidLoginAlert();
                return;
            }
            await Shell.Current.GoToAsync("MainMenuPage", true);
        }
        catch (HttpRequestException ex)
        {
           await  DisplayInvalidLoginAlert();
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    /// <summary>
    /// Helper method to display an alert if the login is invalid
    /// </summary>
    private async Task DisplayInvalidLoginAlert()
    {
        string alertTitle = _localizationService.GetResourceValue("LoginViewModel_InvalidLogin");
        string alertMessage = _localizationService.GetResourceValue("LoginViewModel_InvalidLoginMessage");
        string alertConfirm = _localizationService.GetResourceValue("GeneralOK");
        await Shell.Current.DisplayAlert(alertTitle, alertMessage, alertConfirm);
    }
}
