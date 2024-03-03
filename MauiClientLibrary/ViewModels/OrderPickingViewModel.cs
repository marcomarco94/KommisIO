using System.ComponentModel;

namespace MauiClientLibrary.ViewModels;

[QueryProperty(nameof(PickingOrder), "PickingOrder")]
public partial class OrderPickingViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoapi;
    private readonly IPopupService _popupService;
    public OrderPickingViewModel(
        ILocalizationService localizationService,
        IKommissIOAPI kommissIoApi,
        IPopupService popupService)
    {
        _localizationService = localizationService;
        _kommissIoapi = kommissIoApi;
        _popupService = popupService;
        PropertyChanged += Property_Changed!;
    }
        
    ~OrderPickingViewModel()
    {
        PropertyChanged -= Property_Changed!;
    }
    
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

    [ObservableProperty]  
    bool _articleEnabled;

    [ObservableProperty]  
    bool _amountEnabled;

    [ObservableProperty]  
    bool _stockEnabled = true;
    
    [ObservableProperty]  
    bool _articleEnabledView;

    [ObservableProperty]  
    bool _amountEnabledView;

    [ObservableProperty]  
    bool _stockEnabledView;
    
    [ObservableProperty]
    List<ArticleStockPositions> _validArticles = new();

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
                    x.OrderPosition?.Article.ArticleNumber.ToString() == CurrentArticleNumber);
            var articleToPick = orderToPick?.OrderPosition;
            var stockPosToPick = orderToPick?.StockPosition.FirstOrDefault(x => x.ShelfNumber.ToString() == CurrentShelfNumber);
            int.TryParse(CurrentAmount, out var amount);
            if (articleToPick is not null && stockPosToPick is not null  && amount > 0) {
                pickingResult = await _kommissIoapi.PickAsync(articleToPick, stockPosToPick, amount);
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

    private async Task<bool> ValidateSearchFrame()
    {
        if (!HasErrors)
            return true;

        await Shell.Current.DisplayAlert(_localizationService.GetResourceValue("GeneralError")
            , _localizationService.GetResourceValue("OrderPickingViewModel_IncompleteInput"),
            _localizationService.GetResourceValue("GeneralOK"));
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
                        OrderPosition = await _kommissIoapi.GetPickingOrderPositionByIdAsync(pos.Id),
                        StockPosition =
                            new ObservableCollection<StockPosition>(
                                await _kommissIoapi.GetStockPositionsForArticleAsync(pos.Article))
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
        ValidateArticleNumber();
    }
    
    [RelayCommand]
    private async Task GetStockPositionByScanAsync()
    {
        var scannedBarcode = await _popupService.ShowPopupAsync<ScanPopupViewModel>();
        CurrentShelfNumber = scannedBarcode?.ToString();
        ValidateStockInput();
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
        StockEnabled = true;
        ArticleEnabled = false;
        AmountEnabled = false;
    }

    private void Property_Changed(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsEnabled) 
            || e.PropertyName == nameof(StockEnabled) 
            || e.PropertyName == nameof(ArticleEnabled) 
            || e.PropertyName == nameof(AmountEnabled))
        {
            StockEnabledView = IsEnabled && StockEnabled;
            ArticleEnabledView = IsEnabled && ArticleEnabled;
            AmountEnabledView = IsEnabled && AmountEnabled;
        }
    }

    [RelayCommand]
    private void ValidateStockInput()
    {
        if (OrderPositions == null)
            return;
        
        ValidArticles = (from op in OrderPositions
            where op.StockPosition.Any(sp => sp.ShelfNumber.ToString() == CurrentShelfNumber)
            select op).ToList();
        if (ValidArticles.Any())
        {
            StockEnabled = false;
            ArticleEnabled = true;
        }
        else
        {
            StockEnabled = true;
            ArticleEnabled = false;
        }
    }

    [RelayCommand]
    private void ValidateArticleNumber()
    {
        if (ValidArticles is null || !ValidArticles.Any())
        {
            return;
        }
        ValidArticles = (from op in ValidArticles
            where op.OrderPosition?.Article.ArticleNumber.ToString() == CurrentArticleNumber
            select op).ToList();
        CurrentAmount = ValidArticles.FirstOrDefault().OrderPosition.DesiredAmount.ToString();
        if (ValidArticles.Any())
        {
            ArticleEnabled = false;
            AmountEnabled = true;
        }
        else
        {
            ArticleEnabled = true;
            AmountEnabled = false;
        }
    }
}