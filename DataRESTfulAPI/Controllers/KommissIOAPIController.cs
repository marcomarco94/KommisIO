using BackendDataAccessLayer;
using BackendDataAccessLayer.Entity;
using BackendDataAccessLayer.Repository;
using DataRepoCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataRESTfulAPI.Controllers {
    [ApiController]
    [Route("/api")]
    [Authorize]
    public class KommissIOAPIController : ControllerBase {
        private readonly ILogger<KommissIOAPIController> _logger;
        private readonly IDemoDataBuilder _demoDataBuilder;
        private readonly IPasswordHasher<EmployeeEntity> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IKommissIOUnitOfWork _unitOfWork;

        public KommissIOAPIController(ILogger<KommissIOAPIController> logger, IDemoDataBuilder demoDataBuilder,
            IPasswordHasher<EmployeeEntity> passwordHasher, IConfiguration configuration, IKommissIOUnitOfWork unitOfWork) {
            _logger = logger;
            _demoDataBuilder = demoDataBuilder;
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get the user that is currently loggedIn.
        /// </summary>
        /// <returns>The employee or null if not able to determine.</returns>
        protected async Task<EmployeeEntity?> GetCurrentEmployeeAsync() => await _unitOfWork.EmployeeRepository.GetEmployeeByPersonnelNumberAsync(short.Parse(
            User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));

        /// <summary>
        /// Identify an User by the personnel number. (role: administrator)
        /// </summary>
        /// <param name="personnelNumber">The number identifying the employee.</param>
        /// <returns>Returns the employee if the personnel number exists, otherwise null.</returns>
        [Route("identity/{personnelNumber}")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrator))]
        //[AllowAnonymous]
        public async Task<Employee?> IdentifyEmployeeAysnc(short personnelNumber) {
            return (await _unitOfWork.EmployeeRepository.GetEmployeeByPersonnelNumberAsync(personnelNumber))?.MapToDataModel();
        }

        //Inspired by: https://www.youtube.com/watch?v=KRVjIgr-WOU 1:0:5
        [Route("token/aquire/")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AquireTokenAsync(AuthenticationRequestInformation ari) {
            EmployeeEntity? employee = await _unitOfWork.EmployeeRepository.GetEmployeeByPersonnelNumberAsync(ari.PersonnelNumber);
            var unauthResponse = Unauthorized("Invalid Credentials");

            if (employee is null || !employee.Authenticate(ari.Password, _passwordHasher))
                return unauthResponse;

            var authClaims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, employee.PersonnelNumber.ToString()),
                new Claim("JWTID", Guid.NewGuid().ToString())
            };

            foreach (var role in Enum.GetValues(typeof(Role)))
                if (((Role)employee.Role).HasFlag((Role)role))
                    authClaims.Add(new Claim(ClaimTypes.Role, ((Role)role).ToString()));

            var token = GenerateNewJsonWebToken(authClaims);

            return Ok(token);
        }

        /// <summary>
        /// Get the identity of the user that is identified and authroized.
        /// </summary>
        /// <returns>Returns the employee that is identified and authroized.</returns>
        [Route("identity")]
        [HttpGet]
        [Authorize]
        public async Task<Employee?> GetIdentity() {
            return (await GetCurrentEmployeeAsync())?.MapToDataModel();
        }

        //Inspired by by Dev Empower (2023): https://www.youtube.com/watch?v=KRVjIgr-WOU 1:15:19
        private string GenerateNewJsonWebToken(List<Claim> authClaims) {
#if DEBUG
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ??
                    throw new NullReferenceException("Unable to find configured secret.")));
#else
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("KommissIOJWTSecret") ??
                                throw new NullReferenceException("Unable to find configured secret.")));
#endif

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(12),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256));

            var token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }

        /// <summary>
        /// Get all currently open picking orders. (role: employee, manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of picking orders.</returns>
        [Route("pickingorder/open")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<IEnumerable<PickingOrder>> GetOpenPickingOrdersAsync() {
            return (await _unitOfWork.PickingOrderRepository.WhereAsync(po => po.Employee == null &&
            po.Positions!.Count(pop => (pop.DesiredAmount - pop.PickedAmount) > 0) > 0)).Select(po => po.MapToDataModel());
        }

        /// <summary>
        /// Pick an item from a stock position. (role: employee, administrator)
        /// </summary>
        /// <param name="position">The stock-position from which to pick.</param>
        /// <param name="amount">The amount to pick.</param>
        /// <returns>Return true if the item can be picked.</returns>
        [Route("pick/{orderPosition}/{stockPosition}/{amount}")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Administrator)}")]
        public async Task<bool> PickAsync(int orderPosition, int stockPosition, int amount) {
            var orderPositionEntity = await _unitOfWork.PickingOrderPositionRepository.GetElementByIDAsync(orderPosition);
            if (orderPositionEntity is null) return false;

            var order = await _unitOfWork.PickingOrderRepository.FindAsync(o => o.Positions!.Any(op => op.Id == orderPosition));
            if (order is null) return false;

            var stock = await _unitOfWork.StockPositionRepository.GetElementByIDAsync(stockPosition);
            if (stock is null) return false;

            //Check if the user that is picking the item is assigned.
            var currentEmpPnr = (await GetCurrentEmployeeAsync())?.PersonnelNumber ?? 0;
            if (order.Employee?.PersonnelNumber != currentEmpPnr) return false;

            //make shure that not more objects are picked then in stock.
            if (stock.Amount < amount || stock.Article?.ArticleId != (orderPositionEntity.Article?.ArticleId ?? 0))
                return false;

            orderPositionEntity.PickedAmount += amount;
            stock.Amount -= amount;

            await _unitOfWork.StockPositionRepository.UpdateAsync(stock);
            await _unitOfWork.PickingOrderPositionRepository.UpdateAsync(orderPositionEntity);

            await _unitOfWork.CommitAsync();
            return true;
        }

        /// <summary>
        /// Get all stock positions of an given article (role: employee, manager, administrator)
        /// </summary>
        /// <param name="article">The article for which all its stock positions are requested.</param>
        /// <returns>Returns all stock positons for the article.</returns>
        [Route("stockposition/article/{article}")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<IEnumerable<StockPosition>> GetStockPositionsForArticleAsync(int article) {
            return (await _unitOfWork.StockPositionRepository.WhereAsync(sp => sp.Article!.ArticleId.Equals(article))).Select(article => article.MapToDataModel());
        }

        /// <summary>
        /// Assign the employee to a picking order. (role: employee, administrator)
        /// </summary>
        /// <param name="order">The order to which an employee should be assigned</param>
        /// <returns>True if the employee was successfully assigned.</returns>
        [Route("pickingorder/assign/{order}")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Administrator)}")]
        public async Task<bool> AssignToPickingOrderAsync(int order) {
            var pickingOrder = await _unitOfWork.PickingOrderRepository.GetElementByIDAsync(order);
            if (pickingOrder == null || pickingOrder.Employee != null)
                return false;

            //Assign the current employee registered.
            pickingOrder.Employee = await GetCurrentEmployeeAsync();
            await _unitOfWork.PickingOrderRepository.UpdateAsync(pickingOrder);
            await _unitOfWork.CommitAsync();
            return true;
        }

        /// <summary>
        /// Get all picking orders, even those which are allready assigned to an employee or completed. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all picking-orders.</returns>
        [Route("pickingorder/all")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<IEnumerable<PickingOrder>> GetPickingOrdersAsync() {
            return (await _unitOfWork.PickingOrderRepository.GetElementsAsync()).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Get all picking orders that are finished. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all finished picking orders.</returns>
        [Route("pickingorder/finished")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<IEnumerable<PickingOrder>> GetFinishedPickingOrdersAsync() {
            return (await _unitOfWork.PickingOrderRepository.WhereAsync(po => po.Employee != null &&
            po.Positions!.Count(pop => (pop.DesiredAmount - pop.PickedAmount) > 0) == 0)).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Get all picking orders that are in progress, a employee is assigned but items are still to be picked. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all in progress picking orders.</returns>
        [Route("pickingorder/progress")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<IEnumerable<PickingOrder>> GetInProgressPickingOrdersAsync() {
            return (await _unitOfWork.PickingOrderRepository.WhereAsync(po => po.Employee != null &&
            po.Positions!.Count(pop => (pop.DesiredAmount - pop.PickedAmount) > 0) > 0)).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Get all picking orders that are in progress and assigned to the current user. (role: employee, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all in progress picking orders.</returns>
        [Route("pickingorder/assigned/progress")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Administrator)}")]
        public async Task<IEnumerable<PickingOrder>> GetInProgressAssignedPickingOrdersAsync() {
            var currentEmp = await GetCurrentEmployeeAsync();
            return (await _unitOfWork.PickingOrderRepository.WhereAsync(po => po.Employee != null && po.Employee.PersonnelNumber == currentEmp!.PersonnelNumber &&
            po.Positions!.Count(pop => (pop.DesiredAmount - pop.PickedAmount) > 0) > 0)).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Reset the database to default. (role: administrator)
        /// </summary>
        /// <returns>Returns true if successfull otherwise false.</returns>
        [Route("reset")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrator))]
        //[AllowAnonymous]
        public async Task<bool> ResetToDefaultAsync() {
            return await _demoDataBuilder.BuildDemoDataAsync();
        }

        /// <summary>
        /// Report a damaged article. (role: employee, administrator)
        /// </summary>
        /// <param name="report">The report to file.</param>
        /// <returns>True if the report was successfully filed.</returns>
        [Route("report/damage/fileReport")]
        [HttpPost]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Administrator)}")]
        public async Task<bool> ReportDamagedArticleAsync(DamageReportFileRequest report) {
            var article = await _unitOfWork.ArticleRepository.GetArticleByArticleNumberAsync(report.ArticleNumber);

            if (article is null)
                return false;

            var dmgReport = new DamageReportEntity(0, report.Message);

            dmgReport.Employee = await GetCurrentEmployeeAsync();
            dmgReport.Article = article;

            await _unitOfWork.DamageReportRepository.InsertAsync(dmgReport);
            await _unitOfWork.CommitAsync();

            return true;
        }

        /// <summary>
        /// Get all damage reports. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all damage-reports.</returns>
        [Route("report/damage/all")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<IEnumerable<DamageReport>> GetArticleDamageReportsAsync() {
            return (await _unitOfWork.DamageReportRepository.GetElementsAsync()).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Get the article with the given id. (role:employee, manager, administrator)
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>Return the article with the given id.</returns>
        [Route("article/{id}")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)},{nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<Article?> GetArticleByArticleIdAsync(int id) {
            return (await _unitOfWork.ArticleRepository.FindAsync(a => a.ArticleId == id))?.MapToDataModel();
        }

        /// <summary>
        /// Get the PickingOrder with the given id. (role:employee, manager, administrator)
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>Return the PickingOrder with the given id.</returns>
        [Route("pickingorder/{id}")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<IActionResult> GetPickingOrderByIdAsync(int id) {
            var po = (await _unitOfWork.PickingOrderRepository.FindAsync(a => a.Id == id))?.MapToDataModel();
            if (po is null)
                return NotFound();
            var emp = await GetCurrentEmployeeAsync();
            if (emp?.Role == (byte)Role.Employee && po?.Assignee?.PersonnelNumber != emp?.PersonnelNumber)
                return Unauthorized();
            return Ok(po);
        }

        /// <summary>
        /// Get the PickingOrderPosition with the given id. (role:employee, manager, administrator)
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>Return the PickingOrderPosition with the given id.</returns>
        [Route("pickingorder/position/{id}")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<IActionResult> GetPickingOrderPositionByIdAsync(int id) {
            var po = (await _unitOfWork.PickingOrderRepository.FindAsync(a => a.Positions!.Any(pop=>pop.Id == id)))?.MapToDataModel();
            if(po is null)
                return NotFound();
            var emp = await GetCurrentEmployeeAsync();
            if (emp?.Role == (byte)Role.Employee && po?.Assignee?.PersonnelNumber != emp?.PersonnelNumber)
                return Unauthorized();
            return Ok(po?.OrderPositions.First(pop=>pop.Id == id));
        }

        /// <summary>
        /// Get the article with the given id. (role:employee, manager, administrator)
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>Return the article with the given id.</returns>
        [Route("stockposition/{id}")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)},{nameof(Role.Manager)}, {nameof(Role.Administrator)}")]
        public async Task<StockPosition?> GetStockPositionByIdAsync(int id) {
            return (await _unitOfWork.StockPositionRepository.FindAsync(a => a.Id == id))?.MapToDataModel();
        }

    }
}
