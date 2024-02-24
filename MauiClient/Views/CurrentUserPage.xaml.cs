
namespace MauiClient.Views
{
    public partial class CurrentUserPage : ContentPage
    {
        public CurrentUserPage(CurrentUserViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
