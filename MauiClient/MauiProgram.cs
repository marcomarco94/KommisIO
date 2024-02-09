using DataRepoCore;
using Microsoft.Extensions.Logging;

namespace MauiClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //Register with IOC
            /*
             * Admin & User: personnelNumber = 1, password = adminuser
             * Admin: personnelNumber = 0, password = admin
             * User: personnelNumber = 2, password = user
             */
            builder.Services.AddSingleton<IEmployeeRepository, EmployeeRepositoryDummy>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
