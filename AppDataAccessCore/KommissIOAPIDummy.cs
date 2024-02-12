using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRepoCore {
    public class KommissIOAPIDummy : IKommissIOAPI {

        ///<inheritdoc/>
        public Employee? CurrentEmployee { get; set; }

        /// <summary>
        /// Protected dictionary to dummy the authentication.
        /// </summary>
        protected IList<(Employee employee, string password)> employees;

        /// <summary>
        /// The articles in the database.
        /// </summary>
        protected IList<Article> articles;

        /// <summary>
        /// The picking-orders in the databse
        /// </summary>
        protected IList<PickingOrder> pickingOrders;

        /// <summary>
        /// The damage-reports in the databse
        /// </summary>
        protected IList<DamageReport> damageReports;

        /// <summary>
        /// The stock position in the warehouse.
        /// </summary>
        protected IList<StockPosition> stockPosition;

        protected SemaphoreSlim locker = new SemaphoreSlim(1);

        public KommissIOAPIDummy() {
            var res = RestWithoutAuthAsync().Result;
        }

        ///<inheritdoc/>
        public async Task<Employee?> IdentifyAndAuthenticateAysnc(short personnelNumber, string password) {
            await locker.WaitAsync();

            var emp = employees.FirstOrDefault((e) => e.employee.PersonnelNumber.Equals(personnelNumber) && e.password.Equals(password)).employee;
            if (emp is null) {
                locker.Release();
                return null;
            }
            CurrentEmployee = emp;
            locker.Release();
            return CurrentEmployee;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetOpenPickingOrdersAsync() {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Employee) && !CurrentEmployee.Role.HasFlag(Role.Administrator)))
                throw new UnauthorizedAccessException();

            await locker.WaitAsync();
            var res = pickingOrders.Where(o => o.Assignee is null).ToList();
            locker.Release();
            return res;
        }

        ///<inheritdoc/>
        public async Task<bool> PickAsync(PickingOrderPosition orderPosition, StockPosition position, int? amount = null) {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Employee) && !CurrentEmployee.Role.HasFlag(Role.Administrator)))
                throw new UnauthorizedAccessException();

            await locker.WaitAsync();
            var pos = stockPosition.IndexOf(position);
            stockPosition.Remove(position);
            if (position.Amount - (amount ?? orderPosition.DesiredAmount) < 0) {
                locker.Release();
                throw new ArgumentOutOfRangeException(nameof(position.Amount));
            }
            stockPosition.Insert(pos, new StockPosition() { Id = position.Id, Amount = position.Amount - (amount ?? orderPosition.DesiredAmount), Article = position.Article, ShelfNumber = position.ShelfNumber });
            locker.Release();
            return true;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<StockPosition>> GetStockPositionsForArticleAsync(Article article) {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Employee) && !CurrentEmployee.Role.HasFlag(Role.Manager) && !CurrentEmployee.Role.HasFlag(Role.Administrator)))
                throw new UnauthorizedAccessException();

            await locker.WaitAsync();
            var res = stockPosition.Where(sp => sp.Article.ArticleNumber == article.ArticleNumber).ToList();
            locker.Release();
            return res;
        }

        ///<inheritdoc/>
        public async Task<bool> AssignToPickingOrderAsync(PickingOrder order) {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Employee) && !CurrentEmployee.Role.HasFlag(Role.Administrator)))
                throw new UnauthorizedAccessException();

            if (pickingOrders.FirstOrDefault(o=>o.Id == order.Id).Assignee is not null)
                throw new InvalidOperationException("It is not possible to change the assingee. Only open picking orders can be assinged.");

            await locker.WaitAsync();
            var pos = pickingOrders.IndexOf(order);
            pickingOrders.Remove(order);
            pickingOrders.Insert(pos, new PickingOrder() {Id = order.Id, Assignee = CurrentEmployee, Note = order.Note, OrderPositions = order.OrderPositions, Priority = order.Priority });
            locker.Release();
            return true;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetPickingOrdersAsync() {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Administrator) && !CurrentEmployee.Role.HasFlag(Role.Manager)))
                throw new UnauthorizedAccessException();

            await locker.WaitAsync();
            var li = new List<PickingOrder>(pickingOrders);
            locker.Release();
            return li;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetFinishedPickingOrdersAsync() {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Administrator) && !CurrentEmployee.Role.HasFlag(Role.Manager)))
                throw new UnauthorizedAccessException();

            await locker.WaitAsync();
            var li = pickingOrders.Where(o => o.Assignee is not null).ToList();
            locker.Release();
            return li;
        }

        ///<inheritdoc/>
        public async Task<bool> ResetToDefaultAsync() {
            if (CurrentEmployee is null || !CurrentEmployee.Role.HasFlag(Role.Administrator))
                throw new UnauthorizedAccessException();

            return await RestWithoutAuthAsync();
        }

        protected async Task<bool> RestWithoutAuthAsync() {
            await locker.WaitAsync();
            employees = new List<(Employee employee, string password)>(){
                (employee: new Employee(){ FirstName="Admin", LastName="Musteradmin", PersonnelNumber=1,
                    Role=Role.Administrator }, password: "admin"),
                (employee: new Employee(){ FirstName="Employee", LastName="Musteremployee", PersonnelNumber=2,
                    Role=Role.Employee}, password: "employee"),
                (employee: new Employee(){ FirstName="Manager", LastName="Mustermanager", PersonnelNumber=3,
                    Role=Role.Manager}, password: "manager"),
                (employee: new Employee(){ FirstName="God", LastName="OP", PersonnelNumber=4,
                    Role=Role.Manager | Role.Administrator | Role.Employee}, password: "god")
            };

            articles = new List<Article>() {
                new Article(){Name="Article A", ArticleNumber=1, Description="This is the first article.", Height=100, Length=100, Width=100, Weight=1000},
                new Article(){Name="Article B", ArticleNumber=2, Description="This is the second article.", Height=345, Length=134, Width=32, Weight=245},
                new Article(){Name="Article C", ArticleNumber=3, Description="This is the third article.", Height=1234, Length=32, Width=6546, Weight=4678},
                new Article(){Name="Article D", ArticleNumber=4, Description="This is the fourth article.", Height=345, Length=234, Width=354, Weight=567},
                new Article(){Name="Article E", ArticleNumber=5, Description="This is the fivth article.", Height=456, Length=123, Width=234, Weight=434}
            };


            pickingOrders = new List<PickingOrder>() {
            new PickingOrder(){Id=1,Priority=1, Note="This is a note.", OrderPositions=new List<PickingOrderPosition>(){
                new PickingOrderPosition(){Id = 1,Article=articles[0], DesiredAmount=3, PickedAmount=0},
                new PickingOrderPosition(){Id = 2,Article=articles[1], DesiredAmount=2, PickedAmount=0},
                new PickingOrderPosition(){Id = 3,Article=articles[2], DesiredAmount=6, PickedAmount=0},
                new PickingOrderPosition(){Id = 4,Article=articles[3], DesiredAmount=2, PickedAmount=0},
                new PickingOrderPosition(){Id = 5,Article=articles[4], DesiredAmount=1, PickedAmount=0},
            } },
             new PickingOrder(){Id=2,Priority=2, Note="This is a second note.", OrderPositions=new List<PickingOrderPosition>(){
                new PickingOrderPosition(){Id = 0,Article=articles[3], DesiredAmount=3, PickedAmount=0},
            } },
              new PickingOrder(){Id=3,Priority=1, Note="This is a third note.", OrderPositions=new List<PickingOrderPosition>(){
                new PickingOrderPosition(){Id = 1,Article=articles[0], DesiredAmount=5, PickedAmount=0},
                new PickingOrderPosition(){Id = 2, Article=articles[3], DesiredAmount=3, PickedAmount=0},
            }},
              new PickingOrder(){Id=4,Priority=3, Note="This is a fourth note.", OrderPositions=new List<PickingOrderPosition>(){
                new PickingOrderPosition(){Id = 1, Article=articles[0], DesiredAmount=1, PickedAmount=0},
                new PickingOrderPosition(){Id = 2,Article=articles[3], DesiredAmount=2, PickedAmount=0},
                new PickingOrderPosition(){Id = 3,Article=articles[2], DesiredAmount=2, PickedAmount=0},
            }}
            };

            damageReports = new List<DamageReport>() {
                new DamageReport(){Article=articles[0], Employee=employees[1].employee, Message="This article has been damaged."},
                new DamageReport(){Article=articles[3], Employee=employees[2].employee, Message="This article has been damaged."}
            };


            stockPosition = new List<StockPosition>() {
                new StockPosition(){Id = 1,Article=articles[0], Amount=102, ShelfNumber=301},
                new StockPosition(){Id = 2,Article=articles[1], Amount=23, ShelfNumber=234},
                new StockPosition(){Id = 3,Article=articles[2], Amount=56, ShelfNumber=304},
                new StockPosition(){Id = 4,Article=articles[3], Amount=135, ShelfNumber=220},
                new StockPosition(){Id = 5,Article=articles[4], Amount=3456, ShelfNumber=302},
            };
            locker.Release();
            return true;
        }

        ///<inheritdoc/>
        public async Task<bool> ReportDamagedArticleAsync(DamageReport report) {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Employee) && !CurrentEmployee.Role.HasFlag(Role.Administrator)))
                throw new UnauthorizedAccessException();
            await locker.WaitAsync();
            damageReports.Add(report);
            locker.Release();
            return true;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<DamageReport>> GetArticleDamageReportsAsync() {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Manager) && !CurrentEmployee.Role.HasFlag(Role.Administrator)))
                throw new UnauthorizedAccessException();
            await locker.WaitAsync();
            var li = new List<DamageReport>(damageReports);
            locker.Release();
            return li;
        }

        ///<inheritdoc/>
        public void Dispose() {
            CurrentEmployee = null;
            employees.Clear();
            articles.Clear();
            damageReports.Clear();
            stockPosition.Clear();
            pickingOrders.Clear();
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetInProgressPickingOrdersAsync() {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Administrator) && !CurrentEmployee.Role.HasFlag(Role.Manager)))
                throw new UnauthorizedAccessException();

            await locker.WaitAsync();
            var li = pickingOrders.Where(po=>po.Assignee is not null && 
            po.OrderPositions.Any(op=>op.PickedAmount<op.DesiredAmount)).ToList();
            locker.Release();
            return li;
        }

        ///<inheritdoc/>
        public async Task<IEnumerable<PickingOrder>> GetInProgressAssignedPickingOrdersAsync() {
            if (CurrentEmployee is null || (!CurrentEmployee.Role.HasFlag(Role.Administrator) && !CurrentEmployee.Role.HasFlag(Role.Manager)))
                throw new UnauthorizedAccessException();

            await locker.WaitAsync();
            var li = pickingOrders.Where(po => po.Assignee.Equals(CurrentEmployee) && 
            po.OrderPositions.Any(op => op.PickedAmount < op.DesiredAmount)).ToList();
            locker.Release();
            return li;
        }
    }
}
