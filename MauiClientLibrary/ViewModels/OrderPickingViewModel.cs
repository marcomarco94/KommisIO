using System.ComponentModel;

namespace MauiClientLibrary.ViewModels;

/// <summary>
/// Query property  contains the picking order from the previous page
/// </summary>
[QueryProperty(nameof(PickingOrder), "PickingOrder")]

/// <summary>
/// The OrderPickingViewModel class is responsible for managing the order picking process.
/// </summary>
public partial class OrderPickingViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoapi;
    private readonly IPopupService _popupService;
    
    /// <summary>
    /// Constructor for the OrderPickingViewModel. Set up the necessary services.
    /// </summary>
    /// <param name="localizationService"></param>
    /// <param name="kommissIoApi"></param>
    /// <param name="popupService"></param>
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
        
    /// <summary>
    /// Destructor for the OrderPickingViewModel. Disposes the event handler.
    /// </summary>
    ~OrderPickingViewModel()
    {
        PropertyChanged -= Property_Changed!;
    }
    
    /// <summary>
    /// Property for the picking order
    /// </summary>
    [ObservableProperty] 
    PickingOrder? _pickingOrder;

    /// <summary>
    /// Property for the order positions
    /// </summary>
    [ObservableProperty] 
    ObservableCollection<ArticleStockPositions>? _orderPositions = new();
    
    /// <summary>
    /// Current shelf number property
    /// </summary>
    [ObservableProperty]
    [Required]
    string? _currentShelfNumber;
    
    /// <summary>
    /// Property for the current article number
    /// </summary>
    [ObservableProperty]
    [Required]
    string? _currentArticleNumber;
    
    /// <summary>
    /// Property for the current article amount
    /// </summary>
    [ObservableProperty]
    [Required]
    string? _currentAmount;

    /// <summary>
    /// Property indicated state of the article 
    /// </summary>
    [ObservableProperty]  
    bool _articleEnabled;

    /// <summary>
    /// Property indicated state of the article amount
    /// </summary>
    [ObservableProperty]  
    bool _amountEnabled;

    /// <summary>
    /// Property indicated state of the stock
    /// </summary>
    [ObservableProperty]  
    bool _stockEnabled = true;
    
    /// <summary>
    /// Property sets the visibility state of the article input
    /// </summary>
    [ObservableProperty]  
    bool _articleEnabledView;

    /// <summary>
    /// Property sets the visibility state of the article amount input
    /// </summary>
    [ObservableProperty]  
    bool _amountEnabledView;

    /// <summary>
    /// Property sets the visibility state of the stock input
    /// </summary>
    [ObservableProperty]  
    bool _stockEnabledView;
    
    /// <summary>
    /// Property contains the valid articles
    /// </summary>
    [ObservableProperty]
    List<ArticleStockPositions> _validArticles = new();

    /// <summary>
    /// Command to pick the order
    /// </summary>
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

    /// <summary>
    /// Helper method to validate the search frame
    /// </summary>
    /// <returns></returns>
    private async Task<bool> ValidateSearchFrame()
    {
        if (!HasErrors)
            return true;

        await Shell.Current.DisplayAlert(_localizationService.GetResourceValue("GeneralError")
            , _localizationService.GetResourceValue("OrderPickingViewModel_IncompleteInput"),
            _localizationService.GetResourceValue("GeneralOK"));
        return false;
    }
    
    /// <summary>
    /// Command to get the order positions
    /// </summary>
    [RelayCommand]
    private async Task GetOrderPositionsAsync()
    {
        if (PickingOrder?.OrderPositions != null)
        {
            IsBusy = true;
            if (OrderPositions != null)
            {
                OrderPositions.Clear();
                try
                {
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
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }
      
            IsBusy = false;
        }
        else {
            await Shell.Current.GoToAsync("..");
        }
    }
    
    /// <summary>
    /// Command to get the article by scan
    /// </summary>
    [RelayCommand]
    private async Task GetArticleByScanAsync()
    {
        var scannedBarcode = await _popupService.ShowPopupAsync<ScanPopupViewModel>();
        CurrentArticleNumber = scannedBarcode?.ToString();
        ValidateArticleNumber();
    }
    
    /// <summary>
    /// Command to get the stock position by scan
    /// </summary>
    [RelayCommand]
    private async Task GetStockPositionByScanAsync()
    {
        var scannedBarcode = await _popupService.ShowPopupAsync<ScanPopupViewModel>();
        CurrentShelfNumber = scannedBarcode?.ToString();
        ValidateStockInput();
    }

    /// <summary>
    /// Command to get the Amount by seachInput
    /// </summary>
    /// <param name="amount"></param>
    [RelayCommand]
    private void GetAmountbySearch(string amount)
    {
        CurrentAmount = amount;
    }
    
    /// <summary>
    /// Command to clear the search frame
    /// </summary>
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

    /// <summary>
    /// Event handler for property changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Validates the stock input and set the enabled states
    /// </summary>
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

    /// <summary>
    /// Validates the article number and set the enabled states
    /// </summary>
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