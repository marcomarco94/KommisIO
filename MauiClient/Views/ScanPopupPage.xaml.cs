
namespace MauiClient.Views;

public partial class  ScanPopupPage : Popup
{
    /// <summary>
    /// ScanPopupPage constructor gets the barcode reader options and sets the binding context
    /// </summary>
    /// <param name="viewModel"></param>
    public ScanPopupPage(ScanPopupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        barcodeReader.Options = ScanPopupViewModel.GetBarcodeReaderOptions();
    }
    
    /// <summary>
    /// Close the popup and return the barcode value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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