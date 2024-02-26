using BackendDataAccessLayer.Entity;
using BackendDataAccessLayer.Repository;
using DataRepoCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer {
    public class DemoDataBuilder : IDemoDataBuilder {
        protected IPasswordHasher<EmployeeEntity> _passwordHasher;
        protected IEmployeeRepository _employeeRepository;
        protected IArticleRepository _articleRepository;
        protected IRepository<StockPositionEntity> _stockPositonRepository;
        protected IRepository<PickingOrderEntity> _pickingOrderRepository;
        protected IRepository<PickingOrderPositionEntity> _pickingOrderPositionRepository;
        protected IRepository<DamageReportEntity> _damageReportRepository;

        public DemoDataBuilder(IEmployeeRepository employeeRepository, IPasswordHasher<EmployeeEntity> hasher, IArticleRepository articleRepository,
            IRepository<StockPositionEntity> stockPositonRepository, IRepository<PickingOrderEntity> pickingOrderRepository, IRepository<PickingOrderPositionEntity> pickingOrderPositionRepository,
            IRepository<DamageReportEntity> damageReportRepository) {
            _passwordHasher = hasher;
            _employeeRepository = employeeRepository;
            _articleRepository = articleRepository;
            _damageReportRepository = damageReportRepository;
            _pickingOrderPositionRepository = pickingOrderPositionRepository;
            _pickingOrderRepository = pickingOrderRepository;
            _stockPositonRepository = stockPositonRepository;
        }

        protected static List<Article> articles = new List<Article>() {
                new Article(){ArticleNumber=1, Name="Article A", Description="This is the first article.", Height=100, Length=20, Width=75, Weight=100},
                new Article(){ArticleNumber=2, Name="Article B", Description="This is the second article.", Height=50, Length=100, Width=20, Weight=75},
                new Article(){ArticleNumber=3, Name="Article C", Description="This is the third article.", Height=100, Length=100, Width=100, Weight=50},
                new Article(){ArticleNumber=4, Name="Article D", Description="This is the 4th article.", Height=75, Length=100, Width=50, Weight=100},
                new Article(){ArticleNumber=5, Name="Article E", Description="This is the 5th article.", Height=20, Length=50, Width=20, Weight=100},
                new Article(){ArticleNumber=6, Name="Article F", Description="This is the 6th article.", Height=100, Length=75, Width=100, Weight=50},
                new Article(){ArticleNumber=7, Name="Article G", Description="This is the 8th article.", Height=75, Length=20, Width=100, Weight=100},
                new Article(){ArticleNumber=8, Name="Article H", Description="This is the 9th article.", Height=20, Length=50, Width=75, Weight=100},
            };


        protected static List<Employee> employees = new List<Employee>() {
                new Employee() {FirstName="Andi", LastName="Bond", PersonnelNumber=1, Role=Role.Administrator},
                new Employee() {FirstName="Manual", LastName="Bond", PersonnelNumber=2, Role=Role.Manager},
                new Employee() {FirstName="Eva", LastName="Bond", PersonnelNumber=3, Role=Role.Employee},
                new Employee() {FirstName="Arnold", LastName="Bond", PersonnelNumber=4, Role=Role.Employee},
                new Employee() {FirstName="Daniel", LastName="Bond", PersonnelNumber=5, Role=Role.Default},
                new Employee() {FirstName="James", LastName="Bond", PersonnelNumber=007, Role=Role.Administrator | Role.Manager | Role.Employee},
            };

        protected static List<StockPosition> stockPositions = new List<StockPosition>() {
            new StockPosition(){Amount= 100, Article=articles[0], ShelfNumber=10},
            new StockPosition(){Amount= 456, Article=articles[1], ShelfNumber=34},
            new StockPosition(){Amount= 56, Article=articles[2], ShelfNumber=5},
            new StockPosition(){Amount= 4, Article=articles[3], ShelfNumber=22},
            new StockPosition(){Amount= 456, Article=articles[4], ShelfNumber=345},
            new StockPosition(){Amount= 100, Article=articles[5], ShelfNumber=2},
            new StockPosition(){Amount= 45, Article=articles[6], ShelfNumber=130},
            new StockPosition(){Amount= 64, Article=articles[7], ShelfNumber=54},
            new StockPosition(){Amount= 100, Article=articles[1], ShelfNumber=74},
            new StockPosition(){Amount= 34, Article=articles[3], ShelfNumber=24},
            new StockPosition(){Amount= 56, Article=articles[3], ShelfNumber=56},
            new StockPosition(){Amount= 23, Article=articles[2], ShelfNumber=8},
        };

        protected static List<DamageReport> damageReports = new List<DamageReport>() {
            new DamageReport(){Article=articles[4], Employee=employees[3], Message="This article is damaged."},
            new DamageReport(){Article=articles[5], Employee=employees[3], Message="This article is damaged."},
            new DamageReport(){Article=articles[6], Employee=employees[2], Message="This article is damaged."},
        };

        protected static List<PickingOrder> pickingOrders = new List<PickingOrder>() {
            new PickingOrder() {Assignee=null, Note="This is a note", Priority =3, OrderPositions=new List<PickingOrderPosition>() {
                new PickingOrderPosition(){Article=articles[0], DesiredAmount=2, PickedAmount=0},
                new PickingOrderPosition(){Article=articles[1], DesiredAmount=5, PickedAmount=0},
                new PickingOrderPosition(){Article=articles[2], DesiredAmount=1, PickedAmount=0}
            }
            },
            new PickingOrder() {Assignee=null, Note="This is a other note", Priority =1, OrderPositions=new List<PickingOrderPosition>() {
                new PickingOrderPosition(){Article=articles[2], DesiredAmount=1, PickedAmount=0},
                new PickingOrderPosition(){Article=articles[1], DesiredAmount=2, PickedAmount=0},
            }
            },
            new PickingOrder() { Assignee=employees[3], Note="This is a note", Priority =2, OrderPositions=new List<PickingOrderPosition>() {
                new PickingOrderPosition(){Article=articles[1], DesiredAmount=4, PickedAmount=0},
                new PickingOrderPosition(){Article=articles[5], DesiredAmount=7, PickedAmount=7},
                new PickingOrderPosition(){Article=articles[3], DesiredAmount=2, PickedAmount=0}
            }
            },
            new PickingOrder() { Assignee=null, Note="", Priority =3, OrderPositions=new List<PickingOrderPosition>() {
                new PickingOrderPosition(){Article=articles[0], DesiredAmount=2, PickedAmount=0},
            }
            },
            new PickingOrder() { Assignee=null, Note="This is a note", Priority =2, OrderPositions=new List<PickingOrderPosition>() {
                new PickingOrderPosition(){ Article=articles[7], DesiredAmount=2, PickedAmount=0},
                new PickingOrderPosition(){Article=articles[6], DesiredAmount=5, PickedAmount=0},
                new PickingOrderPosition(){Article=articles[2], DesiredAmount=1, PickedAmount=0}
            }
            },
        };

        /// <inheritdoc/>
        public async Task<bool> BuildDemoDataAsync() {
            bool success = true;

            //Remove the current data.
            success &= await _pickingOrderPositionRepository.ResetAsync();
            success &= await _pickingOrderRepository.ResetAsync();
            success &= await _damageReportRepository.ResetAsync();
            success &= await _stockPositonRepository.ResetAsync();
            success &= await _articleRepository.ResetAsync();
            success &= await _employeeRepository.ResetAsync();

            //Convert the data to entities.

            var entityEmployees = new List<EmployeeEntity>();
            foreach (var employee in employees) {
                var entity = new EmployeeEntity(employee);
                entity.setPassword("password", _passwordHasher); //All users have the password password
                entityEmployees.Add(entity);
            }

            success &= await _employeeRepository.InsertRangeAsync(entityEmployees);

            var entityArticles = new List<ArticleEntity>();
            foreach (var article in articles) {
                var entity = new ArticleEntity(article);
                entityArticles.Add(entity);
            }

            success &= await _articleRepository.InsertRangeAsync(entityArticles);

            var entityStockPositons = new List<StockPositionEntity>();
            foreach (var stockPosition in stockPositions) {
                var entity = new StockPositionEntity(stockPosition);
                entity.Article = await _articleRepository.FindAsync(a => a.ArticleId.Equals(stockPosition.Article.ArticleNumber));
                entityStockPositons.Add(entity);
            }

            success &= await _stockPositonRepository.InsertRangeAsync(entityStockPositons);


            var entityDamageReports = new List<DamageReportEntity>();
            foreach (var damageReport in damageReports) {
                var entity = new DamageReportEntity(damageReport);
                entity.Article = await _articleRepository.FindAsync(entity => entity.ArticleId.Equals(damageReport.Article.ArticleNumber));
                entity.Employee = await _employeeRepository.FindAsync(entity => entity.PersonnelNumber.Equals(damageReport.Employee.PersonnelNumber));
                entityDamageReports.Add(entity);
            }

            success &= await _damageReportRepository.InsertRangeAsync(entityDamageReports);

            var entityPickingOrders = new List<PickingOrderEntity>();
            var entityPickingOrderPositions = new List<PickingOrderPositionEntity>();
            foreach (var pickingOrder in pickingOrders) {
                var entity = new PickingOrderEntity(pickingOrder);
                if (pickingOrder.Assignee is not null)
                    entity.Employee = await _employeeRepository.FindAsync(e => e.PersonnelNumber.Equals(pickingOrder.Assignee.PersonnelNumber));
                var positions = new List<PickingOrderPositionEntity>();
                entity.Positions = positions;

                foreach (var position in pickingOrder.OrderPositions) {
                    var entity1 = new PickingOrderPositionEntity(position);
                    entity1.Article = await _articleRepository.FindAsync(a => a.ArticleId.Equals(position.Article.ArticleNumber));
                    entityPickingOrderPositions.Add(entity1);
                    entity.Positions.Add(entity1);
                }

                entityPickingOrders.Add(entity);
            }

            success &= await _pickingOrderPositionRepository.InsertRangeAsync(entityPickingOrderPositions);
            success &= await _pickingOrderRepository.InsertRangeAsync(entityPickingOrders);

            return success;
        }
    }
}
