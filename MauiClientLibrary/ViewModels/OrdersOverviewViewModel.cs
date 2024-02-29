using System.ComponentModel;

namespace MauiClientLibrary.ViewModels;

public partial class OrdersOverviewViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoapi;
    private readonly IPopupService _popupService;

    public OrdersOverviewViewModel(
        ILocalizationService localizationService, 
        IOrderOverviewStorage orderOverviewStorage,
        IKommissIOAPI kommissIoApi, 
        IPopupService popupService)
    {
        _localizationService = localizationService;
        _kommissIoapi = kommissIoApi;
        _popupService = popupService;

        _activeMenu = orderOverviewStorage.GetActiveMenu();
        PropertyChanged += Property_Changed!;
    }

    ~OrdersOverviewViewModel()
    {
        PropertyChanged -= Property_Changed!;
    }

    [ObservableProperty]
    ObservableCollection<OrderOverviewModel> _activeMenu;

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
    ObservableCollection<PickingOrder>? _pickingOrders = new();

    [ObservableProperty]  
    OrderOverviewModel? _selectedView;

    [RelayCommand]
    private async Task GetPickingOrdersAsync(OrderOverviewModel selectedView)
    {
        SelectedView = selectedView;
        await LoadPickingOrdersAsync();
    }
    
    [RelayCommand]
    private async Task LoadPickingOrdersAsync()
    {
        IsBusy = true;
        PickingOrders?.Clear();
        ObservableCollection<PickingOrder> orders;
        
        if (SelectedView is null)
        {
            orders = new ObservableCollection<PickingOrder>(await _kommissIoapi.GetInProgressAssignedPickingOrdersAsync());
        }
        else
        {
            orders = new ObservableCollection<PickingOrder>(await SelectedView.Function());
        }

        foreach (var order in orders)
        {
            PickingOrders?.Add(order);
        }
        IsBusy = false;
    }
    
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

    private async void Property_Changed(object sender, PropertyChangedEventArgs e)
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

        if (hasError)
        {
            HasErrors = true;
            await Shell.Current.DisplayAlert("Error!", "Invalid barcode", "OK");
            return;
        }

        var orders = await _kommissIoapi.GetOpenPickingOrdersAsync();
        List<PickingOrder> orderList = orders.ToList();
        var foundOrder = orderList.Find(po => po.Id == id);
        if (foundOrder is null)
        {
            HasErrors = true;
            await Shell.Current.DisplayAlert("Error!", "Order not found", "OK");
            return;
        }
        CurrentOrder = foundOrder;
    }

    private async Task CurrentOrderChanged(PickingOrder? value)
    {
        if (value is null) 
            return;

        IsBusy = true;
        if (_kommissIoapi.CurrentEmployee != value.Assignee)
        {
            var canAssign = await _kommissIoapi.AssignToPickingOrderAsync(value);
            if (!canAssign) {
                await Shell.Current.DisplayAlert("Error!", "Order could not be assigned", "OK");
                return;
            }
        }
        else {
            await Shell.Current.GoToAsync("OrderPickingPage", true, new Dictionary<string, object>
            {
                {"PickingOrder", value}
            });
        }
        IsBusy = false;
    }

    private async Task  ErrorChanged(bool value)
    {
        if (value)
        {
            await LoadPickingOrdersAsync();
            HasErrors = false;
        }
    }
    
}
