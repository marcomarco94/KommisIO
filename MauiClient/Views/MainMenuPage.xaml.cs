
namespace MauiClient.Views
{
    public partial class MainMenuPage : ContentPage
    {
        public MainMenuPage(MainMenuViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
