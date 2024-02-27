namespace MauiClient.Views;
public partial class OrdersOverviewPage : ContentPage
{
    public OrdersOverviewPage(OrdersOverviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}