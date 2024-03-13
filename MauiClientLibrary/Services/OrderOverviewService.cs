
namespace MauiClientLibrary.Services
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class OrderOverviewService : IOrderOverviewService
    {
        private readonly List<OrderOverviewModel> _activeMenu;
        private readonly IKommissIOAPI _kommissIoApi;
        private readonly ILocalizationService _localizationService;

        /// <summary>
        /// Constructor for the OrderOverviewService sets up the necessary services
        /// </summary>
        /// <param name="kommissIoApi"></param>
        /// <param name="localizationService"></param>
        public OrderOverviewService(IKommissIOAPI kommissIoApi, ILocalizationService localizationService)
        {
            _kommissIoApi = kommissIoApi;
            _localizationService = localizationService;
            _activeMenu = CreateMenuList();
        }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
        public ObservableCollection<OrderOverviewModel> GetActiveMenu()
        {
            var employeeRole = _kommissIoApi.CurrentEmployee!.Role;
            return new ObservableCollection<OrderOverviewModel>(_activeMenu.Where(r => HasAnyRole(r.RequiredRole, employeeRole)));
        }

        /// <summary>
        ///  Checks if the user has the required role
        /// </summary>
        /// <param name="requiredRoles"></param>
        /// <param name="employeeRole"></param>
        /// <returns></returns>
        private bool HasAnyRole(Role requiredRoles, Role employeeRole)
        {
            return requiredRoles.HasFlag(employeeRole) || employeeRole.HasFlag(requiredRoles);
        }

        /// <summary>
        /// Creates the menu list with the required roles and the function to call
        /// </summary>
        /// <returns></returns>
        public List<OrderOverviewModel> CreateMenuList()
        {
            return new List<OrderOverviewModel>
            {
                new()
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_AllOrders"), RequiredRole = Role.Manager | Role.Administrator, Function = () => _kommissIoApi.GetPickingOrdersAsync()
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_OpenOrders"), RequiredRole = Role.Employee | Role.Manager | Role.Administrator, Function = () => _kommissIoApi.GetOpenPickingOrdersAsync()
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_InProgress"), RequiredRole =  Role.Administrator | Role.Manager, Function = () => _kommissIoApi.GetInProgressPickingOrdersAsync()
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_AssignedInProgress"), RequiredRole =  Role.Administrator | Role.Employee, Function = () => _kommissIoApi.GetInProgressAssignedPickingOrdersAsync()
                },
                new()
                {
                    Title = _localizationService.GetResourceValue("OrderOverviewStorage_CompletedOrders"), RequiredRole = Role.Manager | Role.Administrator, Function = () => _kommissIoApi.GetFinishedPickingOrdersAsync()
                }
            };
        }
    }
}