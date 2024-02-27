
namespace MauiClientLibrary.Storages
{
    public class MainMenuStorage : IMainMenuStorage
    {
        private readonly List<MenuItemModel> _activeMenu;
        private readonly IKommissIOAPI _kommissIoApi;

        public MainMenuStorage(IKommissIOAPI kommissIoApi)
        {
            _kommissIoApi = kommissIoApi;
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
                    Title = "Wareneingang", Icon = "incoming_goods.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee
                },
                new MenuItemModel
                    { Title = "Einlagerung", Icon = "storage.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee },
                new MenuItemModel
                {
                    Title = "Kommissionierung", Icon = "order_picking.png", Route = "OrdersOverviewPage", RequiredRole = Role.Employee
                },
                new MenuItemModel
                {
                    Title = "Warenausgang", Icon = "outgoing_goods.png", Route = "UnderConstructionPage", RequiredRole = Role.Employee
                },
                new MenuItemModel
                {
                    Title = "Inventur", Icon = "inventory.png", Route = "UnderConstructionPage", RequiredRole = Role.Manager
                },
                new MenuItemModel
                {
                    Title = "Einstellungen", Icon = "settings.png", Route = "UnderConstructionPage", RequiredRole = Role.Administrator
                }
            };
        }
    }
}