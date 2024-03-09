using System.ComponentModel;

namespace MauiClientLibrary.ViewModels;

/// <summary>
/// OrdersOverviewViewModel provides the overview of the orders
/// </summary>
public partial class OrdersOverviewViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoapi;
    private readonly IPopupService _popupService;

    /// <summary>
    /// Constructor for the OrdersOverviewViewModel sets up the necessary services
    /// </summary>
    /// <param name="localizationService"></param>
    /// <param name="orderOverviewService"></param>
    /// <param name="kommissIoApi"></param>
    /// <param name="popupService"></param>
    public OrdersOverviewViewModel(
        ILocalizationService localizationService, 
        IOrderOverviewService orderOverviewService,
        IKommissIOAPI kommissIoApi, 
        IPopupService popupService)
    {
        _localizationService = localizationService;
        _kommissIoapi = kommissIoApi;
        _popupService = popupService;
        _activeMenu = orderOverviewService.GetActiveMenu();
        PropertyChanged += Property_Changed!;
    }

    /// <summary>
    /// Destructor for the OrdersOverviewViewModel disposes the event handler
    /// </summary>
    ~OrdersOverviewViewModel()
    {
        PropertyChanged -= Property_Changed!;
    }

    /// <summary>
    /// Property for the active menu
    /// </summary>
    [ObservableProperty]
    ObservableCollection<OrderOverviewModel> _activeMenu;

    /// <summary>
    /// Property for the barcode value
    /// </summary>
    [ObservableProperty] 
    string? _barcodeValue;
    
    /// <summary>
    /// Property for the current order
    /// </summary>
    [ObservableProperty]
    PickingOrder? _currentOrder;

    /// <summary>
    /// Property if the view has errors
    /// </summary>
    [ObservableProperty] 
    bool _hasErrors = true;
    
    /// <summary>
    /// Property for the search id
    /// </summary>
    [ObservableProperty] 
    string? _searchId;

    /// <summary>
    /// Property for the selected order
    /// </summary>
    [ObservableProperty] 
    PickingOrder? _selectedOrder;
    
    /// <summary>
    /// Property for the picking orders
    /// </summary>
    [ObservableProperty]  
    ObservableCollection<PickingOrder>? _pickingOrders = new();

    /// <summary>
    /// Property for the selected view
    /// </summary>
    [ObservableProperty]  
    OrderOverviewModel? _selectedView;

    /// <summary>
    /// Command to get the picking orders for the selected view
    /// </summary>
    /// <param name="selectedView"></param>
    [RelayCommand]
    private async Task GetPickingOrdersAsync(OrderOverviewModel selectedView)
    {
        SelectedView = selectedView;
        await LoadPickingOrdersAsync();
    }
    
    /// <summary>
    /// Command to get the picking orders
    /// </summary>
    [RelayCommand]
    private async Task LoadPickingOrdersAsync()
    {
        IsBusy = true;
        PickingOrders?.Clear();
        ObservableCollection<PickingOrder> orders;

        try
        {
            if (SelectedView is null)
            {
                orders = new ObservableCollection<PickingOrder>(await _kommissIoapi.GetOpenPickingOrdersAsync());
            }
            else
            {
                orders = new ObservableCollection<PickingOrder>(await SelectedView.Function());
            }

            foreach (var order in orders)
            {
                PickingOrders?.Add(order);
            }
        }
        catch (HttpRequestException ex)
        {
                await Shell.Current.DisplayAlert(_localizationService.GetResourceValue("GeneralError"),
                    _localizationService.GetResourceValue("OrdersOverviewViewModel_PermissionError"),
                    _localizationService.GetResourceValue("GeneralOK"));
        }
        finally
        {
            IsBusy = false;
        }

    }
    
    /// <summary>
    /// Command to get the barcode by scan
    /// </summary>
    [RelayCommand]
    private async Task  GetBarcodeByScanAsync()
    {
        var scannedBarcode = await _popupService.ShowPopupAsync<ScanPopupViewModel>();
        BarcodeValue = scannedBarcode?.ToString();
    }

    /// <summary>
    /// Command to get the barcode by search
    /// </summary>
    /// <param name="search"></param>
    [RelayCommand]
    private void  GetBarcodeBySearch(string search)
    {
        BarcodeValue = search;
    }
    
    /// <summary>
    /// Command to get the order by selection
    /// </summary>
    [RelayCommand]
    private void GetOrderBySelection()
    {
        if (SelectedOrder is null)
            return;
        CurrentOrder = SelectedOrder;
    }

    /// <summary>
    /// Event handler for the property changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
    
    /// <summary>
    /// Command to handle the barcode value changed
    /// </summary>
    /// <param name="value"></param>
    private async Task BarcodeValueChanged(string? value)
    {
        SearchId = String.Empty;
        SelectedOrder = null;
        bool hasError = !int.TryParse(value, out int id);

        if (hasError)
        {
            HasErrors = true;
            await Shell.Current.DisplayAlert(_localizationService.GetResourceValue("GeneralError"), 
                _localizationService.GetResourceValue("OrdersOverviewViewModel_InvalidBarcode"),
                _localizationService.GetResourceValue("GeneralOK"));
            return;
        }

        var orders = await _kommissIoapi.GetOpenPickingOrdersAsync();
        List<PickingOrder> orderList = orders.ToList();
        var foundOrder = orderList.Find(po => po.Id == id);
        if (foundOrder is null)
        {
            HasErrors = true;
            await Shell.Current.DisplayAlert(_localizationService.GetResourceValue("GeneralError"),
                _localizationService.GetResourceValue("OrdersOverviewViewModel_OrderNotFound"),
                _localizationService.GetResourceValue("GeneralOK"));
            return;
        }
        CurrentOrder = foundOrder;
    }

    /// <summary>
    /// Command to handle the current order changed
    /// </summary>
    /// <param name="value"></param>
    private async Task CurrentOrderChanged(PickingOrder? value)
    {
        if (value is null) 
            return;

        IsBusy = true;
        
        bool isAssigned = _kommissIoapi.CurrentEmployee == value.Assignee;
        
        if (!isAssigned)
        {
            isAssigned = await _kommissIoapi.AssignToPickingOrderAsync(value);
        }
        if (!isAssigned) {
            await Shell.Current.DisplayAlert(_localizationService.GetResourceValue("GeneralError"),
                _localizationService.GetResourceValue("OrdersOverviewViewModel_CantAssign"),
                _localizationService.GetResourceValue("GeneralOK"));
               IsBusy = false;
            return;
        }
        await Shell.Current.GoToAsync("OrderPickingPage", true, new Dictionary<string, object>
        {
            {"PickingOrder", value}
        });
        IsBusy = false;
    }

    /// <summary>
    /// Helper method to handle the error changed
    /// </summary>
    /// <param name="value"></param>
    private async Task  ErrorChanged(bool value)
    {
        if (value)
        {
            await LoadPickingOrdersAsync();
            HasErrors = false;
        }
    }
    
}
