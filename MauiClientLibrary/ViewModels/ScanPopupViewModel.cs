
using ZXing.Net.Maui;

namespace MauiClientLibrary.ViewModels;

/// <summary>
/// ScanPopupViewModel funtionality for the scan popup
/// </summary>
public partial class ScanPopupViewModel : BaseViewModel
{
    /// <summary>
    /// Gets the barcode reader options
    /// </summary>
    /// <returns></returns>
    public static BarcodeReaderOptions GetBarcodeReaderOptions()
    {
        return new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.Code39,
            AutoRotate = true,
            Multiple = false,
        };
    }
}