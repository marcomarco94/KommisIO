using MauiClient.Views;

namespace MauiClient
{
    public partial class AppShell : Shell
    {
        public AppShell(NavBarViewModel navBarViewModel)
        {
            InitializeComponent();
            BindingContext = navBarViewModel;
            
            Routing.RegisterRoute(nameof(MainMenuPage), typeof(MainMenuPage));
            Routing.RegisterRoute(nameof(CurrentUserPage), typeof(CurrentUserPage));
            Routing.RegisterRoute(nameof(UnderConstructionPage), typeof(UnderConstructionPage));
            Routing.RegisterRoute(nameof(OrdersOverviewPage), typeof(OrdersOverviewPage));
            Routing.RegisterRoute(nameof(OrderPickingPage), typeof(OrderPickingPage)); 
        }
    }
}
