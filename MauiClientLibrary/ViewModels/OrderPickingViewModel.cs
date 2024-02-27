using System.Collections.ObjectModel;

namespace MauiClientLibrary.ViewModels;

[QueryProperty(nameof(PickingOrder), "PickingOrder")]
public partial class OrderPickingViewModel : BaseViewModel
{
    private readonly ILocalizationService _localizationService;
    private readonly IKommissIOAPI _kommissIoApi;

    public OrderPickingViewModel(ILocalizationService localizationService, IKommissIOAPI kommissIoApi)
    {
        _localizationService = localizationService;
        _kommissIoApi = kommissIoApi;
    }
    
    [ObservableProperty] 
    PickingOrder? _pickingOrder;

    [ObservableProperty] 
    ObservableCollection<ArticleStockPositions>? _orderPositions = new();
    
    [ObservableProperty]
    ArticleStockPositions? _selectedOrderPosition;
    
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
    
}