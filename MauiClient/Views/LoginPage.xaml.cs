
namespace MauiClient.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
        
        /// <summary>
        /// OnNavigatedTo event handler
        /// </summary>
        /// <param name="args"></param>
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            (BindingContext as LoginViewModel)?.LogOutCommand.Execute(null);
        }
    }
}
