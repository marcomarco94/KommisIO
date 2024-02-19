
using System.ComponentModel;

namespace MauiClientLibrary.ViewModels;
public partial class BaseViewModel : ObservableValidator
{ 
    public BaseViewModel()
    {
        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
    ~BaseViewModel()
    {
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
    }

    void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        IsConnected = e.NetworkAccess == NetworkAccess.Internet;
    }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotConnected))]
    [NotifyPropertyChangedFor(nameof(IsEnabled))]
    bool _isConnected;
    
    public bool IsNotConnected => !IsConnected;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    [NotifyPropertyChangedFor(nameof(IsEnabled))]
    private bool _isBusy;
    
    public bool IsNotBusy => !IsBusy;
    public bool IsEnabled => IsNotBusy && IsConnected;

    [ObservableProperty] 
    bool _isLoggedIn;
    public bool IsLoggedOut => !IsLoggedIn;
}