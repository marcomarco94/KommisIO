
namespace MauiClientLibrary.Services
{
    /// <summary>
     /// <inheritdoc/>
    /// </summary>
    public class MainMenuService : IMainMenuService
    {
        private readonly List<MenuItemModel> _activeMenu;
        private readonly IKommissIOAPI _kommissIoApi;
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Constructor for the MainMenuService sets up the necessary services
        /// </summary>
        /// <param name="kommissIoApi"></param>
        /// <param name="localizationService"></param>
        public MainMenuService(IKommissIOAPI kommissIoApi, ILocalizationService localizationService)
        {
            _kommissIoApi = kommissIoApi;
            _localizationService = localizationService;
            _activeMenu = CreateMainMenu();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ObservableCollection<MenuItemModel> GetMainMenu()
        {
            var employeeRole = _kommissIoApi.CurrentEmployee!.Role;
            return new ObservableCollection<MenuItemModel>(_activeMenu.Where(r => HasAnyRole(r.RequiredRole, employeeRole)));
        }
        
        /// <summary>
        /// Checks if the user has the required role
        /// </summary>
        /// <param name="requiredRoles"></param>
        /// <param name="employeeRole"></param>
        /// <returns></returns>
        private bool HasAnyRole(Role requiredRoles, Role employeeRole)
        {
            return requiredRoles.HasFlag(employeeRole) || employeeRole.HasFlag(requiredRoles);
        }

        /// <summary>
        /// Creates the main menu list with the required roles and the function to call
        /// </summary>
        /// <returns></returns>
        private List<MenuItemModel> CreateMainMenu()
        {
            return new List<MenuItemModel>
            {
                new()
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_IncomingGoods"), Icon = "incoming_goods.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee | Role.Manager | Role.Administrator
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_Storage"), Icon = "storage.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee | Role.Manager | Role.Administrator
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_OrderPicking"), Icon = "order_picking.png", Route = "OrdersOverviewPage", RequiredRole = Role.Employee | Role.Manager | Role.Administrator
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_OutgoingGoods"), Icon = "outgoing_goods.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee | Role.Manager | Role.Administrator
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_Inventory"), Icon = "inventory.png", Route = "UnderConstructionPage", RequiredRole = Role.Manager | Role.Administrator
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_Settings"), Icon = "settings.png", Route = "UnderConstructionPage", RequiredRole = Role.Administrator
                }
            };
        }
    }
}
