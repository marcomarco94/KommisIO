
namespace MauiClient.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            (BindingContext as LoginViewModel)?.LogOutCommand.Execute(null);
        }
    }
}
