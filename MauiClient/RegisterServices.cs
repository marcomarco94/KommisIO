


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

            /*
             * Users: 
             * PersonnelNumber = 1, Role = Role.Administrator, password: "admin",
             * PersonnelNumber = 2, Role = Role.Employee password: "employee",
             * PersonnelNumber = 3, Role = Role.Manager}, password: "manager",
             * PersonnelNumber=4, Role=Role.Manager | Role.Administrator | Role.Employee, password: "god
            */
            builder.Services.AddSingleton<IKommissIOAPI, KommissIOAPI>();
        }
    }
}
