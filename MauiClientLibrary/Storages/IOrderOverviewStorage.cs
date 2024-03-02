namespace MauiClientLibrary.Storages;

public interface IOrderOverviewStorage
{
    ObservableCollection<OrderOverviewModel> GetActiveMenu();
}