namespace MauiClientLibrary.Services;

/// <summary>
/// OrderOverviewService provides the functions for the order overview
/// </summary>
public interface IOrderOverviewService
{
    /// <summary>
    /// Gets the active menu for the current user based on the role
    /// </summary>
    /// <returns></returns>
    ObservableCollection<OrderOverviewModel> GetActiveMenu();
}