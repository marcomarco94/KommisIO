using MauiClient.Views;

namespace MauiClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainMenuPage), typeof(MainMenuPage));
        }
    }
}
