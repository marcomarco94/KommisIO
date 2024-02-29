using System.ComponentModel;

namespace MauiClientLibrary.ViewModels;

[QueryProperty(nameof(PickingOrder), "PickingOrder")]
public partial class OrderPickingViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoApi;
    private readonly IPopupService _popupService;

    public OrderPickingViewModel(ILocalizationService localizationService, IKommissIOAPI kommissIoApi, IPopupService popupService)
    {
        _localizationService = localizationService;
        _kommissIoApi = kommissIoApi;
        _popupService = popupService;
    }
    
    [ObservableProperty] 
    PickingOrder? _pickingOrder;

    [ObservableProperty] 
    ObservableCollection<ArticleStockPositions>? _orderPositions = new();
    
    [ObservableProperty]
    string? _currentShelfNumber;
    
    [ObservableProperty]
    string? _currentArticleNumber;
    
    [ObservableProperty]
    string? _currentAmount;

    [RelayCommand]
    private async Task PickOrderAsync()
    {
        bool pickingResult = false;
        if (OrderPositions != null)
        {
            var orderToPick =
                OrderPositions.FirstOrDefault(x =>
                    x.OrderPosition.Article.ArticleNumber.ToString() == CurrentArticleNumber);
            var articleToPick = orderToPick?.OrderPosition;
            var stockPosToPick = orderToPick?.StockPosition.FirstOrDefault(x => x.ShelfNumber.ToString() == CurrentShelfNumber);
            int.TryParse(CurrentAmount, out var amount);
            if (articleToPick is not null && stockPosToPick is not null  && amount > 0) {
                pickingResult = await _kommissIoApi.PickAsync(articleToPick, stockPosToPick, amount);
            }
        }
        if (pickingResult)
        {
            IsBusy = true;
            await Shell.Current.DisplayAlert("Success", "Erfolgreichl", "OK");
            await GetOrderPositionsAsync();
            ClearSearchFrame();
            IsBusy = false;

        }
        else {
            Shell.Current.DisplayAlert("Error", "Fehler", "OK");
        }
    }
    
    
    [RelayCommand]
    private async Task GetOrderPositionsAsync()
    {
       
        if (PickingOrder?.OrderPositions != null)
        {
            IsBusy = true;
            if (OrderPositions != null)
            {
                OrderPositions.Clear();
                foreach (var pos in PickingOrder.OrderPositions)
                {
                    OrderPositions.Add(new ArticleStockPositions
                    {
                        OrderPosition = pos,
                        StockPosition =
                            new ObservableCollection<StockPosition>(
                                await _kommissIoApi.GetStockPositionsForArticleAsync(pos.Article))
                    });
                }
            }

            IsBusy = false;
        }
        else {
            await Shell.Current.GoToAsync("..");
        }
    }
    
    [RelayCommand]
    private async Task GetArticleByScanAsync()
    {
        var scannedBarcode = await _popupService.ShowPopupAsync<ScanPopupViewModel>();
        CurrentArticleNumber = scannedBarcode?.ToString();
    }
    
    [RelayCommand]
    private async Task GetStockPositionByScanAsync()
    {
        var scannedBarcode = await _popupService.ShowPopupAsync<ScanPopupViewModel>();
        CurrentShelfNumber = scannedBarcode?.ToString();
    }

    [RelayCommand]
    private void GetAnmountbySearch(string amount)
{
        CurrentAmount = amount;
    }
    
    [RelayCommand]
    public void ClearSearchFrame()
    {
        CurrentShelfNumber = string.Empty;
        CurrentArticleNumber = string.Empty;
        CurrentAmount = string.Empty;
    }
}