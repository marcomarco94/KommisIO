namespace MauiClient.Views;
public partial class OrdersOverviewPage : ContentPage
{
    public OrdersOverviewPage(OrdersOverviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    /// <summary>
    /// OnAppearing event handler
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as OrdersOverviewViewModel)?.LoadPickingOrdersCommand.ExecuteAsync("false");
    }
}