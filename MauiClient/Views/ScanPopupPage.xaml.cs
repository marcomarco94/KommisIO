
namespace MauiClient.Views;

public partial class  ScanPopupPage : Popup
{
    public ScanPopupPage(ScanPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        barcodeReader.Options = ScanPopupViewModel.GetBarcodeReaderOptions();
    }
    
    private void BarcodeReader_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var first = e.Results.FirstOrDefault();
        if (first is null)
            return;

        Dispatcher.DispatchAsync(async () =>
        {
            await CloseAsync(first.Value);
        });
        
    }
}