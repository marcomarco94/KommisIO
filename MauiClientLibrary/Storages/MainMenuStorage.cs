
namespace MauiClientLibrary.Storages
{
    public class MainMenuStorage : IMainMenuStorage
    {
        private readonly List<MenuItemModel> _activeMenu;
        private readonly IKommissIOAPI _kommissIoApi;
        private readonly ILocalizationService _localizationService;

        public MainMenuStorage(IKommissIOAPI kommissIoApi, ILocalizationService localizationService)
        {
            _kommissIoApi = kommissIoApi;
            _localizationService = localizationService;
            _activeMenu = CreateMainMenu();
        }

        public ObservableCollection<MenuItemModel> GetMainMenu()
        {
            var employeeRole = _kommissIoApi.CurrentEmployee!.Role;
            return new ObservableCollection<MenuItemModel>(_activeMenu.Where(r => Enum.Parse<Role>(r.RequiredRole.ToString()) >= employeeRole));
            
        }

        private List<MenuItemModel> CreateMainMenu()
        {
            return new List<MenuItemModel>
            {
                new MenuItemModel
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_IncomingGoods"), Icon = "incoming_goods.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee
                },
                new MenuItemModel
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_Storage"), Icon = "storage.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee
                },
                new MenuItemModel
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_OrderPicking"), Icon = "order_picking.png", Route = "OrdersOverviewPage", RequiredRole = Role.Employee
                },
                new MenuItemModel
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_OutgoingGoods"), Icon = "outgoing_goods.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee
                },
                new MenuItemModel
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_Inventory"), Icon = "inventory.png", Route = "UnderConstructionPage", RequiredRole = Role.Manager
                },
                new MenuItemModel
                {
                    Title = _localizationService.GetResourceValue("MainMenuSotrage_Settings"), Icon = "settings.png", Route = "UnderConstructionPage", RequiredRole = Role.Administrator
                }
            };
        }
    }
}
