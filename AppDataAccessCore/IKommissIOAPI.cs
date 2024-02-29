using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public interface IKommissIOAPI : IDisposable {

        /// <summary>
        /// The employee that is currently authenticated.
        /// </summary>
        public Employee? CurrentEmployee { get; set; }

        /// <summary>
        /// Identify an User by the personnel number and password. (role: anonymous, default, any)
        /// </summary>
        /// <param name="personnelNumber">The number identifying the employee.</param>
        /// <param name="password">The password</param>
        /// <returns>Returns the employee if the personnel number exists and the password is correct, otherwise null.</returns>
        public Task<Employee?> IdentifyAndAuthenticateAysnc(short personnelNumber, string password);

        /// <summary>
        /// Get all currently open picking orders. (role: employee, manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of picking orders.</returns>
        public Task<IEnumerable<PickingOrder>> GetOpenPickingOrdersAsync();

        /// <summary>
        /// Pick an item from a stock position. (role: employee, administrator)
        /// </summary>
        /// <param name="orderPosition">The order-position for which to pick.</param>
        /// <param name="position">The stock-position from which to pick.</param>
        /// <param name="amount">The amount to pick.</param>
        /// <returns>Return true if the item can be picked.</returns>
        public Task<bool> PickAsync(PickingOrderPosition orderPosition, StockPosition position, int? amount = null);

        /// <summary>
        /// Get all stock positions of an given article (role: employee, manager, administrator)
        /// </summary>
        /// <param name="article">The article for which all its stock positions are requested.</param>
        /// <returns></returns>
        public Task<IEnumerable<StockPosition>> GetStockPositionsForArticleAsync(Article article);

        /// <summary>
        /// Assign the employee to a picking order. (role: employee, administrator)
        /// </summary>
        /// <param name="order">The order to which an employee should be assigned</param>
        /// <returns>True if the employee was successfully assigned.</returns>
        public Task<bool> AssignToPickingOrderAsync(PickingOrder order);

        /// <summary>
        /// Get all picking orders, even those which are allready assigned to an employee or completed. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all picking-orders.</returns>
        public Task<IEnumerable<PickingOrder>> GetPickingOrdersAsync();

        /// <summary>
        /// Get all picking orders that are finished. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all finished picking orders.</returns>
        public Task<IEnumerable<PickingOrder>> GetFinishedPickingOrdersAsync();

        /// <summary>
        /// Get all picking orders that are in progress, a employee is assigned but items are still to be picked. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all in progress picking orders.</returns>
        public Task<IEnumerable<PickingOrder>> GetInProgressPickingOrdersAsync();

        /// <summary>
        /// Get all picking orders that are in progress and assigned to the current user. (role: employee, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all in progress picking orders.</returns>
        public Task<IEnumerable<PickingOrder>> GetInProgressAssignedPickingOrdersAsync();

        /// <summary>
        /// Reset the database to default. (role: administrator)
        /// </summary>
        /// <returns>Returns true if successfull otherwise false.</returns>
        public Task<bool> ResetToDefaultAsync();

        /// <summary>
        /// Report a damaged article. (role: employee, administrator)
        /// </summary>
        /// <param name="article">The article to file.</param>
        /// <param name="message">The message to include in the fileing.</param>
        /// <returns>True if the report was successfully filed.</returns>
        public Task<bool> ReportDamagedArticleAsync(Article article, string message);

        /// <summary>
        /// Get all damage reports. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all damage-reports.</returns>
        public Task<IEnumerable<DamageReport>> GetArticleDamageReportsAsync();
    }
}
