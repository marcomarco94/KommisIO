


using AppDataAccessCore;
using CommunityToolkit.Maui;

namespace MauiClient
{
    public static class RegisterServices
    {
        public static void ConfigureService(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            builder.Services.AddSingleton<IMainMenuStorage, MainMenuStorage>();
            builder.Services.AddSingleton<IOrderOverviewStorage, OrderOverviewStorage>();

            builder.Services.AddSingleton<UnderConstructionPage>();
            builder.Services.AddSingleton<NavBarViewModel>();
            builder.Services.AddTransient<CurrentUserPage>();
            builder.Services.AddTransient<CurrentUserViewModel>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<MainMenuPage>();
            builder.Services.AddTransient<MainMenuViewModel>();
            builder.Services.AddTransient<OrdersOverviewPage>();
            builder.Services.AddTransient<OrdersOverviewViewModel>();
            builder.Services.AddTransient<OrderPickingPage>();
            builder.Services.AddTransient<OrderPickingViewModel>();
            builder.Services.AddTransientPopup<ScanPopupPage, ScanPopupViewModel>();

            
            //builder.Services.AddSingleton<IKommissIOAPI>(x=>new KommissIOAPI("https://kommissio.azurewebsites.net/api/"));

            builder.Services.AddSingleton<IKommissIOAPI, KommissIOAPIDummy>();
        }
    }
}
