
namespace MauiClient.Views;

public partial class OrderPickingPage : ContentPage
{
    public OrderPickingPage(OrderPickingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}