
namespace MauiClientLibrary.ViewModels;

[QueryProperty(nameof(PickingOrder), "PickingOrder")]
public partial class OrderPickingViewModel(
    ILocalizationService localizationService,
    IKommissIOAPI kommissIoApi,
    IPopupService popupService)
    : BaseViewModel
{
    private readonly ILocalizationService _localizationService = localizationService;

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
                pickingResult = await kommissIoApi.PickAsync(articleToPick, stockPosToPick, amount);
            }
        }
        if (pickingResult)
        {
            IsBusy = true;
            await Shell.Current.DisplayAlert(_localizationService.GetResourceValue("OrderPickingViewModel_Success")
                ,_localizationService.GetResourceValue("OrderPickingViewModel_Success"), 
                _localizationService.GetResourceValue("GeneralOK"));
            await GetOrderPositionsAsync();
        }
        else {
            await Shell.Current.DisplayAlert(_localizationService.GetResourceValue("GeneralError")
                ,_localizationService.GetResourceValue("GeneralError"), 
                _localizationService.GetResourceValue("GeneralOK"));
        }
        ClearSearchFrame();
        IsBusy = false;
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
                                await kommissIoApi.GetStockPositionsForArticleAsync(pos.Article))
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
        var scannedBarcode = await popupService.ShowPopupAsync<ScanPopupViewModel>();
        CurrentArticleNumber = scannedBarcode?.ToString();
    }
    
    [RelayCommand]
    private async Task GetStockPositionByScanAsync()
    {
        var scannedBarcode = await popupService.ShowPopupAsync<ScanPopupViewModel>();
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