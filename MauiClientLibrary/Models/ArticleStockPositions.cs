namespace MauiClientLibrary.Models;

public class ArticleStockPositions
{
    public PickingOrderPosition? OrderPosition { get; set; }
    public ObservableCollection<StockPosition> StockPosition { get; set; } = new ();
}