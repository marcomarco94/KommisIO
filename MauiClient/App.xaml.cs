
namespace MauiClient
{
    public partial class App : Application
    {
        public App(NavBarViewModel navBarViewModel)
        {
            InitializeComponent();
            
            MainPage = new AppShell(navBarViewModel);
        }
    }
}
