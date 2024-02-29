
namespace MauiClientLibrary.Storages
{
    public class OrderOverviewStorage : IOrderOverviewStorage
    {
        private readonly List<OrderOverviewModel> _activeMenu;
        private readonly IKommissIOAPI _kommissIoApi;

        public OrderOverviewStorage(IKommissIOAPI kommissIoApi)
        {
            _kommissIoApi = kommissIoApi;
            _activeMenu = CreateMenuList();
        }

        public ObservableCollection<OrderOverviewModel> GetActiveMenu()
        {
            var employeeRole = _kommissIoApi.CurrentEmployee!.Role;
            return new ObservableCollection<OrderOverviewModel>(_activeMenu.Where(r => Enum.Parse<Role>(r.RequiredRole.ToString()) >= employeeRole));
        }

        public List<OrderOverviewModel> CreateMenuList()
        {
            return new List<OrderOverviewModel>
            {
  
                new OrderOverviewModel
                {
                    Title = "Alle Aufträge", RequiredRole = Role.Manager, Function = () => _kommissIoApi.GetPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = "Offene Aufträge", RequiredRole = Role.Employee, Function = () => _kommissIoApi.GetOpenPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = "Zugeordnet in Bearbeitung", RequiredRole = Role.Employee, Function = () => _kommissIoApi.GetInProgressAssignedPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = "Abgeschlossene Aufträge", RequiredRole = Role.Manager, Function = () => _kommissIoApi.GetInProgressAssignedPickingOrdersAsync()
                }
            };
        }
    }
}