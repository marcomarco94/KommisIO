
using DataRepoCore;
using MauiClient.Views;
using MauiClientLibrary.Services;

namespace MauiClient
{
    public static class RegisterServices
    {
        public static void ConfigureService(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
            
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<MainMenuPage>();
            builder.Services.AddSingleton<MainMenuViewModel>();
            

            /*
             * Users: 
             * PersonnelNumber = 1, Role = Role.Administrator, password: "admin",
             * PersonnelNumber = 2, Role = Role.Employee password: "employee",
             * PersonnelNumber = 3, Role = Role.Manager}, password: "manager",
             * PersonnelNumber=4, Role=Role.Manager | Role.Administrator | Role.Employee, password: "god
            */
            builder.Services.AddSingleton<IKommissIOAPI, KommissIOAPIDummy>();
        }
    }
}
