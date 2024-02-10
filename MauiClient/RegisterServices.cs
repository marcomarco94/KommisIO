
using DataRepoCore;
using MauiClient.Views;

namespace MauiClient
{
    public static class RegisterServices
    {
        public static void ConfigureService(this MauiAppBuilder builder)
        {

            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<LoginViewModel>();

            //Register with IOC
            /*
             * Admin & User: personnelNumber = 1, password = adminuser
             * Admin: personnelNumber = 0, password = admin
             * User: personnelNumber = 2, password = user
             */
            builder.Services.AddSingleton<IKommissIOAPI, KommissIOAPIDummy>();
        }
    }
}
