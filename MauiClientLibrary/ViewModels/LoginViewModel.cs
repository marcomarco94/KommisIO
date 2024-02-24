﻿
namespace MauiClientLibrary.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IKommissIOAPI _kommissIoApi;
    private readonly ILocalizationService _localizationService;
    
    public LoginViewModel( IKommissIOAPI kommissIoApi, ILocalizationService localizationService)
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
    private async Task LoginAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            var validationResults = GetErrors();
            string errorMessages = string.Join(Environment.NewLine, validationResults);
            string alertTitle = _localizationService.GetResourceValue("InvalidInput");
            string alertConfirm = _localizationService.GetResourceValue("Ok");
            await Shell.Current.DisplayAlert(alertTitle, errorMessages, alertConfirm);
            return;
        }

        short personnelNumberParsed = 0;
        bool canParse = short.TryParse(PersonnelNumber, out personnelNumberParsed);
        var currentEmployee = await _kommissIoApi.IdentifyAndAuthenticateAysnc(personnelNumberParsed, Password);
        if (currentEmployee.HasValue)
        {
            await Shell.Current.GoToAsync("MainMenuPage", true,
                new Dictionary<string, object>
                {
                   { "currentEmployee", currentEmployee },
                });
        }
        else 
        {
            string alertTitle = _localizationService.GetResourceValue("InvalidLogin");
            string alertMessage = _localizationService.GetResourceValue("InvalidLoginMessage");
            string alertConfirm = _localizationService.GetResourceValue("Ok");
            await Shell.Current.DisplayAlert(alertTitle, alertMessage, alertConfirm);
        }
    }
}