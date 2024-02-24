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
        private readonly IRepository<PickingOrderEntity> _pickingOrderRepository;
        private readonly IRepository<DamageReportEntity> _damageReportRepository;
        private readonly IRepository<StockPositionEntity> _stockPostitionRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IRepository<PickingOrderPositionEntity> _pickingOrderPositionRepository;
        private readonly IConfiguration _configuration;

        public KommissIOAPIController(ILogger<KommissIOAPIController> logger, IDemoDataBuilder demoDataBuilder,
            IRepository<PickingOrderEntity> pickingOrderRepository, IRepository<PickingOrderPositionEntity> pickingOrderPositionRepository,
            IRepository<StockPositionEntity> stockPostitionRepository, IRepository<DamageReportEntity> damageReportRepository,
            IArticleRepository articleRepository, IEmployeeRepository employeeRepository, IPasswordHasher<EmployeeEntity> passwordHasher,
            IConfiguration configuration) {
            _logger = logger;
            _demoDataBuilder = demoDataBuilder;
            _pickingOrderRepository = pickingOrderRepository;
            _stockPostitionRepository = stockPostitionRepository;
            _damageReportRepository = damageReportRepository;
            _articleRepository = articleRepository;
            _employeeRepository = employeeRepository;
            _passwordHasher = passwordHasher;
            _pickingOrderPositionRepository = pickingOrderPositionRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Get the user that is currently loggedIn.
        /// </summary>
        /// <returns>The employee or null if not able to determine.</returns>
        protected async Task<EmployeeEntity?> GetCurrentEmployeeAsync() => await _employeeRepository.GetEmployeeByPersonnelNumberAsync(short.Parse(
            User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));

        /// <summary>
        /// Identify an User by the personnel number. (role: administrator)
        /// </summary>
        /// <param name="personnelNumber">The number identifying the employee.</param>
        /// <returns>Returns the employee if the personnel number exists, otherwise null.</returns>
        [Route("identify/{personnelNumber}")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrator))]
        public async Task<Employee?> IdentifyEmployeeAysnc(short personnelNumber) {
            return (await _employeeRepository.GetEmployeeByPersonnelNumberAsync(personnelNumber))?.MapToDataModel();
        }

        //Inspired by: https://www.youtube.com/watch?v=KRVjIgr-WOU 1:0:5
        [Route("token/aquire/")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AquireTokenAsync(AuthenticationRequestInformation ari) {
            EmployeeEntity? employee = await _employeeRepository.GetEmployeeByPersonnelNumberAsync(ari.PersonnelNumber);
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

        //Inspired by by Dev Empower (2023): https://www.youtube.com/watch?v=KRVjIgr-WOU 1:15:19
        private string GenerateNewJsonWebToken(List<Claim> authClaims) {
#if DEBUG
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ??
                    throw new NullReferenceException("Unable to find configured secret.")));
#else
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("KommissIOJWTSecret") ??
                                throw new NullReferenceException("Unable to find configured secret.")))
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
        /// Get all currently open picking orders. (role: employee)
        /// </summary>
        /// <returns>Return an enumerable of picking orders.</returns>
        [Route("pickingorder/open")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Employee))]
        public async Task<IEnumerable<PickingOrder>> GetOpenPickingOrdersAsync() {
            return (await _pickingOrderRepository.WhereAsync(po => po.Employee == null &&
            po.Positions!.Count(pop => (pop.DesiredAmount - pop.PickedAmount) > 0) > 0)).Select(po => po.MapToDataModel());
        }

        /// <summary>
        /// Pick an item from a stock position. (role: employee)
        /// </summary>
        /// <param name="position">The stock-position from which to pick.</param>
        /// <param name="amount">The amount to pick.</param>
        /// <returns>Return true if the item can be picked.</returns>
        [Route("pick/{orderPosition}/{stockPosition}/{amount}")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Employee))]
        public async Task<bool> PickAsync(int orderPosition, int stockPosition, int amount) {
            var orderPositionEntity = await _pickingOrderPositionRepository.GetElementByIDAsync(orderPosition);
            if (orderPositionEntity is null) return false;

            var order = await _pickingOrderRepository.FindAsync(o => o.Positions!.Any(op => op.Id == orderPosition));
            if (order is null) return false;

            var stock = await _stockPostitionRepository.GetElementByIDAsync(stockPosition);
            if (stock is null) return false;

            //Check if the user that is picking the item is assigned.
            var currentEmpPnr = (await GetCurrentEmployeeAsync())?.PersonnelNumber ?? 0;
            if (order.Employee?.PersonnelNumber != currentEmpPnr) return false;

            //make shure that not more objects are pick then in stock.
            //This check is not thread-safe, it does not need to be. The Range Condition is set, but it is better to just check not thread-safe before 
            //the entity framework / databse check is performed.
            if (stock.Amount < amount || stock.Article?.ArticleId != (orderPositionEntity.Article?.ArticleId ?? 0))
                return false;
            orderPositionEntity.PickedAmount += amount;
            stock.Amount -= amount;

            return await _stockPostitionRepository.UpdateAsync(stock) &&
                await _pickingOrderPositionRepository.UpdateAsync(orderPositionEntity);
        }

        /// <summary>
        /// Get all stock positions of an given article (role: employee, manager)
        /// </summary>
        /// <param name="article">The article for which all its stock positions are requested.</param>
        /// <returns>Returns all stock positons for the article.</returns>
        [Route("stockposition/{article}")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Employee)}, {nameof(Role.Manager)}")]
        public async Task<IEnumerable<StockPosition>> GetStockPositionsForArticleAsync(int article) {
            return (await _stockPostitionRepository.WhereAsync(sp => sp.Article!.ArticleId.Equals(article))).Select(article => article.MapToDataModel());
        }

        /// <summary>
        /// Assign the employee to a picking order. (role: employee)
        /// </summary>
        /// <param name="order">The order to which an employee should be assigned</param>
        /// <returns>True if the employee was successfully assigned.</returns>
        [Route("pickingorder/assign/{order}")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Employee))]
        public async Task<bool> AssignToPickingOrderAsync(int order) {
            var pickingOrder = await _pickingOrderRepository.GetElementByIDAsync(order);
            if (pickingOrder == null)
                return false;

            //Assign the current employee registered.
            pickingOrder.Employee = await GetCurrentEmployeeAsync();
            return await _pickingOrderRepository.UpdateAsync(pickingOrder);
        }

        /// <summary>
        /// Get all picking orders, even those which are allready assigned to an employee or completed. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all picking-orders.</returns>
        [Route("pickingorder")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Administrator)}, {nameof(Role.Manager)}")]
        public async Task<IEnumerable<PickingOrder>> GetPickingOrdersAsync() {
            return (await _pickingOrderRepository.GetElementsAsync()).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Get all picking orders that are finished. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all finished picking orders.</returns>
        [Route("pickingorder/finished")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Administrator)}, {nameof(Role.Manager)}")]
        public async Task<IEnumerable<PickingOrder>> GetFinishedPickingOrdersAsync() {
            return (await _pickingOrderRepository.WhereAsync(po => po.Employee != null &&
            po.Positions!.Count(pop => (pop.DesiredAmount - pop.PickedAmount) > 0) == 0)).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Get all picking orders that are in progress, a employee is assigned but items are still to be picked. (role: manager, administrator)
        /// </summary>
        /// <returns>Return an enumerable of all in progress picking orders.</returns>
        [Route("pickingorder/progress")]
        [HttpGet]
        [Authorize(Roles = $"{nameof(Role.Administrator)}, {nameof(Role.Manager)}")]
        public async Task<IEnumerable<PickingOrder>> GetInProgressPickingOrdersAsync() {
            return (await _pickingOrderRepository.WhereAsync(po => po.Employee != null &&
            po.Positions!.Count(pop => (pop.DesiredAmount - pop.PickedAmount) > 0) > 0)).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Get all picking orders that are in progress and assigned to the current user. (role: employee)
        /// </summary>
        /// <returns>Return an enumerable of all in progress picking orders.</returns>
        [Route("pickingorder/assigned/progress")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Employee))]
        public async Task<IEnumerable<PickingOrder>> GetInProgressAssignedPickingOrdersAsync() {
            var currentEmp = await GetCurrentEmployeeAsync();
            return (await _pickingOrderRepository.WhereAsync(po => po.Employee != null && po.Employee.PersonnelNumber == currentEmp!.PersonnelNumber &&
            po.Positions!.Count(pop => (pop.DesiredAmount - pop.PickedAmount) > 0) > 0)).Select(e => e.MapToDataModel());
        }

        /// <summary>
        /// Reset the database to default. (role: administrator)
        /// </summary>
        /// <returns>Returns true if successfull otherwise false.</returns>
        [Route("reset")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Administrator))]
        public async Task<bool> ResetToDefaultAsync() {
            return await _demoDataBuilder.BuildDemoDataAsync();
        }

        /// <summary>
        /// Report a damaged article. (role: employee)
        /// </summary>
        /// <param name="report">The report to file.</param>
        /// <returns>True if the report was successfully filed.</returns>
        [Route("report/damage/fileReport")]
        [HttpPost]
        [Authorize(Roles = nameof(Role.Employee))]
        public async Task<bool> ReportDamagedArticleAsync(int articleNumber, string message) {
            var article = await _articleRepository.GetArticleByArticleNumberAsync(articleNumber);

            if (article is null)
                return false;

            var dmgReport = new DamageReportEntity(0, message);

            dmgReport.Employee = await GetCurrentEmployeeAsync();
            dmgReport.Article = article;

            return await _damageReportRepository.InsertAsync(dmgReport);
        }

        /// <summary>
        /// Get all damage reports. (role: manager)
        /// </summary>
        /// <returns>Return an enumerable of all damage-reports.</returns>
        [Route("report/damage/all")]
        [HttpGet]
        [Authorize(Roles = nameof(Role.Manager))]
        public async Task<IEnumerable<DamageReport>> GetArticleDamageReportsAsync() {
            return (await _damageReportRepository.GetElementsAsync()).Select(e => e.MapToDataModel());
        }
    }
}
