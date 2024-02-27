using System.ComponentModel;

namespace MauiClientLibrary.ViewModels;

public partial class OrdersOverviewViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoapi;
    private readonly IPopupService _popupService;

    public OrdersOverviewViewModel(ILocalizationService localizationService, IKommissIOAPI kommissIoApi, 
        IPopupService popupService)
    {
        _localizationService = localizationService;
        _kommissIoapi = kommissIoApi;
        _popupService = popupService;
        PropertyChanged += Property_Changed;
    }

    ~OrdersOverviewViewModel()
    {
        PropertyChanged -= Property_Changed;
    }
    
    [ObservableProperty] 
    string? _barcodeValue;
    
    [ObservableProperty]
    PickingOrder? _currentOrder;

    [ObservableProperty] 
    bool _hasErrors = true;
    
    [ObservableProperty] 
    string? _searchId;

    [ObservableProperty] 
    PickingOrder? _selectedOrder;
    
    [ObservableProperty]  
    ObservableCollection<PickingOrder> _pickingOrders = new();
    
    [ObservableProperty]
    string _filterInProgress;
    
    [RelayCommand]
    private async Task  GetBarcodeByScanAsync()
    {
        var scannedBarcode = await _popupService.ShowPopupAsync<ScanPopupViewModel>();
        BarcodeValue = scannedBarcode?.ToString();
    }

    [RelayCommand]
    private void  GetBarcodeBySearch(string search)
    {
        BarcodeValue = search;
    }
    
    [RelayCommand]
    private void GetOrderBySelection()
    {
        if (SelectedOrder is null)
            return;
        CurrentOrder = SelectedOrder;
    }

    public async void Property_Changed(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BarcodeValue))
        {
            await BarcodeValueChanged(BarcodeValue);
        }
        if (e.PropertyName == nameof(CurrentOrder))
        {
            await CurrentOrderChanged(CurrentOrder);
        }
        if (e.PropertyName == nameof(HasErrors))
        {
            await ErrorChanged(HasErrors);
        }
    }
    private async Task BarcodeValueChanged(string? value)
    {
        SearchId = String.Empty;
        SelectedOrder = null;
        bool hasError = !int.TryParse(value, out int id);
        PickingOrder? foundOrder = null;

        if (hasError)
        {
            HasErrors = true;
            await Shell.Current.DisplayAlert("Error!", "Invalid barcode", "OK");
            return;
        }

        var orders = await _kommissIoapi.GetOpenPickingOrdersAsync();
        List<PickingOrder> orderList = orders.ToList();
        foundOrder = orderList.Find(po => po.Id == id);
        if (foundOrder is null)
        {
            HasErrors = true;
            Shell.Current.DisplayAlert("Error!", "Order not found", "OK");
            return;
        }
        CurrentOrder = foundOrder;
    }

    private async Task CurrentOrderChanged(PickingOrder? value)
    {
        if (value is null) 
            return;

        var canAssign = await _kommissIoapi.AssignToPickingOrderAsync(value);
        if (!canAssign) {
            HasErrors = true;
            await Shell.Current.DisplayAlert("Error!", "Order could not be assigned", "OK");
            await  LoadPickingOrdersAsync(FilterInProgress);
            return;
        }
        
        await Shell.Current.GoToAsync("OrderPickingPage", true, new Dictionary<string, object>
        {
            {"PickingOrder", value}
        });
    }

    private async Task  ErrorChanged(bool value)
    {
        if (value)
        {
            await LoadPickingOrdersAsync(FilterInProgress);
            HasErrors = false;
        }
    }

    [RelayCommand]
    private async Task LoadPickingOrdersAsync(string inProgress)
    {
        IsBusy = true;
        FilterInProgress = inProgress;
        try
        {
            PickingOrders.Clear();
            IEnumerable<PickingOrder> orders;
            if (inProgress == "true") { 
                orders = await _kommissIoapi.GetInProgressPickingOrdersAsync();
            }
            else {
                orders = await _kommissIoapi.GetOpenPickingOrdersAsync();
            }
            foreach (var order in orders)
            {
                PickingOrders.Add(order);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get orders: {ex.Message}");
            await Shell.Current.DisplayAlert("Error!", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
}
