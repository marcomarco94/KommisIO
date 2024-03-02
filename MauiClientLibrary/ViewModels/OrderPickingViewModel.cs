
namespace MauiClientLibrary.ViewModels;

[QueryProperty(nameof(PickingOrder), "PickingOrder")]
public partial class OrderPickingViewModel(
    ILocalizationService localizationService,
    IKommissIOAPI kommissIoApi,
    IPopupService popupService)
    : BaseViewModel
{
    [ObservableProperty] 
    PickingOrder? _pickingOrder;

    [ObservableProperty] 
    ObservableCollection<ArticleStockPositions>? _orderPositions = new();
    
    [ObservableProperty]
    [Required]
    string? _currentShelfNumber;
    
    [ObservableProperty]
    [Required]
    string? _currentArticleNumber;
    
    [ObservableProperty]
    [Required]
    string? _currentAmount;

    [RelayCommand]
    private async Task PickOrderAsync()
    {
        if (await ValidateSearchFrame() == false)
            return;
        
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
            await Shell.Current.DisplayAlert(localizationService.GetResourceValue("OrderPickingViewModel_Success")
                ,localizationService.GetResourceValue("OrderPickingViewModel_Success"), 
                localizationService.GetResourceValue("GeneralOK"));
            await GetOrderPositionsAsync();
        }
        else {
            await Shell.Current.DisplayAlert(localizationService.GetResourceValue("GeneralError")
                ,localizationService.GetResourceValue("GeneralError"), 
                localizationService.GetResourceValue("GeneralOK"));
        }

        ClearSearchFrame();
        IsBusy = false;
    }

    private async Task<bool> ValidateSearchFrame()
    {
        if (!HasErrors)
            return true;

        await Shell.Current.DisplayAlert(localizationService.GetResourceValue("GeneralError")
            , localizationService.GetResourceValue("OrderPickingViewModel_IncompleteInput"),
            localizationService.GetResourceValue("GeneralOK"));
        return false;
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
    private void GetAmountbySearch(string amount)
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