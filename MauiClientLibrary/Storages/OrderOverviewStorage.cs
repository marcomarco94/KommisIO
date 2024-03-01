
namespace MauiClientLibrary.Storages
{
    public class OrderOverviewStorage : IOrderOverviewStorage
    {
        private readonly List<OrderOverviewModel> _activeMenu;
        private readonly IKommissIOAPI _kommissIoApi;
        private readonly ILocalizationService _localizationService;

        public OrderOverviewStorage(IKommissIOAPI kommissIoApi, ILocalizationService localizationService)
        {
            _kommissIoApi = kommissIoApi;
            _localizationService = localizationService;
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
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_AllOrders"), RequiredRole = Role.Manager, Function = () => _kommissIoApi.GetPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_OpenOrders"), RequiredRole = Role.Employee, Function = () => _kommissIoApi.GetOpenPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_AssignedInProgress"), RequiredRole = Role.Employee, Function = () => _kommissIoApi.GetInProgressAssignedPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_CompletedOrders"), RequiredRole = Role.Manager, Function = () => _kommissIoApi.GetInProgressAssignedPickingOrdersAsync()
                }
            };
        }
    }
}