
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
    }

    [ObservableProperty] 
    string? _barcodeValue;

    [ObservableProperty]  
    ObservableCollection<PickingOrder> _pickingOrders = new();
    
    [ObservableProperty] 
    PickingOrder _selectedOrder;

    [ObservableProperty] 
    string _searchText;
    
    [ObservableProperty]
    bool _barCodeScannerVisible = false;

    [RelayCommand]
    private async Task LoadPickingOrdersAsync(string inProgress)
    {
        IsBusy = true;
        try
        {
            PickingOrders.Clear();
            IEnumerable<PickingOrder> orders;
            if (inProgress == "true") { 
                orders = await _kommissIoapi.GetInProgressPickingOrdersAsync();
            }
            else {
                orders = await _kommissIoapi.GetPickingOrdersAsync();
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
    
    [RelayCommand]
    private async Task  GetBarcodeByScanAsync()
    {
        var scanned = await _popupService.ShowPopupAsync<ScanPopupViewModel>();
        BarcodeValue = scanned?.ToString();
    }
    
    [RelayCommand]
    private async Task  ValidateBarcodeAsync(string barcode)
    {
        bool canParse = int.TryParse(barcode, out int id);
        PickingOrder? foundOrder = null;
        
        if (canParse)
        {
            var orders = await _kommissIoapi.GetOpenPickingOrdersAsync();
            List<PickingOrder> orderList = orders.ToList();
            foundOrder = orderList.Find(po => po.Id == id);

        }

        if (foundOrder is not null)
        {
                await _kommissIoapi.AssignToPickingOrderAsync(foundOrder);
                Shell.Current.GoToAsync("OrderPickingPage", true);
        }
        
        
    }
}
