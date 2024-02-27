
namespace MauiClient.Views;

public partial class OrderPickingPage : ContentPage
{
    public OrderPickingPage(OrderPickingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
       
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        (BindingContext as OrderPickingViewModel)?.GetOrderPositionsCommand.ExecuteAsync(null);
    }
}