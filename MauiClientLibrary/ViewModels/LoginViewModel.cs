
namespace MauiClientLibrary.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IKommissIOAPI _kommissIoApi;
    private readonly ILocalizationService _localizationService;
    
    public LoginViewModel(IKommissIOAPI kommissIoApi, ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        _kommissIoApi = kommissIoApi;
    }
    
    [Required]
    [Range(0, 9999)]
    [ObservableProperty]
    string _personnelNumber = string.Empty;

    [Required]
    [MinLength(4)]
    [MaxLength(30)]
    [ObservableProperty]
    string _password = string.Empty;

    [ObservableProperty]
    bool  _isPasswordValid;

    [ObservableProperty]
    bool _isPersonnelNumberValid;
    
    [RelayCommand]
    private void ValidateEntries()
    {
        ValidateAllProperties();
        IsPersonnelNumberValid = !GetErrors(nameof(PersonnelNumber)).Any();
        IsPasswordValid = !GetErrors(nameof(Password)).Any();
    }

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
    
    private async Task DisplayInvalidLoginAlert()
    {
        string alertTitle = _localizationService.GetResourceValue("LoginViewModel_InvalidLogin");
        string alertMessage = _localizationService.GetResourceValue("LoginViewModel_InvalidLoginMessage");
        string alertConfirm = _localizationService.GetResourceValue("GeneralOK");
        await Shell.Current.DisplayAlert(alertTitle, alertMessage, alertConfirm);
    }
}
