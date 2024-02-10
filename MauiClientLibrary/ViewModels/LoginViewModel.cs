
using DataRepoCore;

namespace MauiClientLibrary.ViewModels;

public partial class LoginViewModel : BaseViewModel
{
    private readonly IKommissIOAPI _kommissIoApi;
    public LoginViewModel(IKommissIOAPI kommissIoApi)
    {
        _kommissIoApi = kommissIoApi;
    }

    [Required]
    [ObservableProperty]
    short _personnelNumber;

    [Required]
    [MinLength(4)]
    [ObservableProperty]
    string _password = string.Empty;


    [RelayCommand]
    async Task LoginAsync()
    {
        
        ValidateAllProperties();
        if (HasErrors)
        {
            await Shell.Current.DisplayAlert("Error", "Blabla", "OK");
            return;
        }

        IsBusy = true;
        var currentEmployee = await _kommissIoApi.IdentifyAndAuthenticateAysnc(PersonnelNumber, Password);
        IsBusy = false;

        //await Shell.Current.GoToAsync(nameof(MainMenuPage), true,
        //    new Dictionary<string, object>
        //    {
        //       // { "currentEmployee", currentEmployee },
        //    });
    }


}