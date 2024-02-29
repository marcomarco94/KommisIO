
using ZXing.Net.Maui;

namespace MauiClientLibrary.ViewModels;

public partial class ScanPopupViewModel : BaseViewModel
{
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