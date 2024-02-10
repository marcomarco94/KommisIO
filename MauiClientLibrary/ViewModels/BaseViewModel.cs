using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiClientLibrary.ViewModels;
public partial class BaseViewModel : ObservableValidator
{
    public BaseViewModel()
    {
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }
    
    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        // Not implemented
    }
    
    [ObservableProperty]
    bool isConnected;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;
    
    public bool IsNotBusy => !IsBusy;

}
