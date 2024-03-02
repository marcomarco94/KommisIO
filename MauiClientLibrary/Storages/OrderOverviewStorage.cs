
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
            return new ObservableCollection<OrderOverviewModel>(_activeMenu.Where(r => HasAnyRole(r.RequiredRole, employeeRole)));
        }

        private bool HasAnyRole(Role requiredRoles, Role employeeRole)
        {
            return requiredRoles.HasFlag(employeeRole) || employeeRole.HasFlag(requiredRoles);
        }

        public List<OrderOverviewModel> CreateMenuList()
        {
            return new List<OrderOverviewModel>
            {
                new OrderOverviewModel
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_AllOrders"), RequiredRole = Role.Manager | Role.Administrator, Function = () => _kommissIoApi.GetPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_OpenOrders"), RequiredRole = Role.Employee | Role.Manager | Role.Administrator, Function = () => _kommissIoApi.GetOpenPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_AssignedInProgress"), RequiredRole =  Role.Manager | Role.Administrator, Function = () => _kommissIoApi.GetInProgressAssignedPickingOrdersAsync()
                },
                new OrderOverviewModel
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_CompletedOrders"), RequiredRole = Role.Employee | Role.Administrator, Function = () => _kommissIoApi.GetInProgressAssignedPickingOrdersAsync()
                }
            };
        }
    }
}