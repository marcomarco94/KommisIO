
using System.ComponentModel;

namespace MauiClientLibrary.ViewModels;

/// <summary>
/// The base view model. Provides basic functionalities.
/// </summary>
public partial class BaseViewModel : ObservableValidator
{ 
    public BaseViewModel()
    {
        Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
        IsConnected = Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
    
    /// <summary>
    /// Destructor removes the event handler.
    /// </summary>
    ~BaseViewModel()
    {
        Connectivity.ConnectivityChanged -= Connectivity_ConnectivityChanged;
    }
    
    /// <summary>
    /// Event handler for connectivity changes. Updates the IsConnected state.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="e">The event arguments containing the connectivity change details.</param>
    void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
    {
        IsConnected = e.NetworkAccess == NetworkAccess.Internet;
    }
    
    /// <summary>
    /// IsConnected property. Indicated if the Device is connected to the internet.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotConnected))]
    [NotifyPropertyChangedFor(nameof(IsEnabled))]
    [NotifyPropertyChangedFor(nameof(IsNotEnabled))]
    bool _isConnected;
    
    /// <summary>
    /// IsNotConnected. Counterpart of IsConnected.
    /// </summary>
    public bool IsNotConnected => !IsConnected;
    
    /// <summary>
    /// IsBusy property. Indicates if the view model is in an operation
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    [NotifyPropertyChangedFor(nameof(IsEnabled))]
    [NotifyPropertyChangedFor(nameof(IsNotEnabled))]
    private bool _isBusy;
    
    /// <summary>
    /// Counterpart of IsBusy
    /// </summary>
    public bool IsNotBusy => !IsBusy;
    
    /// <summary>
    /// IsEnabled property. Indicates if the view model is not busy and connected to the internet.
    /// </summary>
    public bool IsEnabled => IsNotBusy && IsConnected;
    
    /// <summary>
    ///  Counterpart of IsEnabled
    /// </summary>
    public bool IsNotEnabled => !IsNotBusy && !IsConnected;

    /// <summary>
    /// IsLoggedIn property. Indicates if the user is logged in.
    /// </summary>
    [ObservableProperty] 
    bool _isLoggedIn;
    
}