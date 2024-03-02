using AppDataAccessCore;
using DataRepoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTests {
    public class KommissIOAPITests {
        //Test on your local server, start it and set the uri here.
        readonly string uri = "https://localhost:7051/api/"; /*"https://kommissio.azurewebsites.net/api/";*/

        [Fact]
        public async Task TestAuthentication() {
            KommissIOAPI api = new KommissIOAPI(uri);
            Employee? e = await api.IdentifyAndAuthenticateAysnc(007, "password");
            Assert.NotNull(e);
        }

        [Fact]
        public async Task TestReset() {
            //Authenticate the user.
            KommissIOAPI api = new KommissIOAPI(uri);
            Employee? e = await api.IdentifyAndAuthenticateAysnc(007, "password");
            Assert.NotNull(e);

            Assert.True(await api.ResetToDefaultAsync());
        }

        protected async Task<IKommissIOAPI> SetupKommissIOAPI() {
            KommissIOAPI api = new KommissIOAPI(uri);
            Employee? e = await api.IdentifyAndAuthenticateAysnc(007, "password");
            Assert.NotNull(e);

            Assert.True(await api.ResetToDefaultAsync());

            return api;
        }

        [Fact]
        public async Task TestGetOpenPickingOrders() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            Assert.NotEmpty(await api.GetOpenPickingOrdersAsync());
        }

        [Fact]
        public async Task TestGetStockPosition() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            var pos = await api.GetOpenPickingOrdersAsync();
            Assert.NotNull(pos);
            Assert.NotEmpty(pos);

            var pops = pos.First().OrderPositions;
            Assert.NotNull(pops);
            Assert.NotEmpty(pops);

            var pop = pops.First();
            var spos = await api.GetStockPositionsForArticleAsync(pop.Article);

            Assert.NotNull(spos);
            Assert.NotEmpty(spos);
        }

        [Fact]
        public async Task TestAssignEmployeeToPickingOrder() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            var pos = await api.GetOpenPickingOrdersAsync();
            Assert.NotNull(pos);
            Assert.NotEmpty(pos);

            var po = pos.First();
            Assert.NotNull(po);

            Assert.Empty(await api.GetInProgressAssignedPickingOrdersAsync());

            Assert.True(await api.AssignToPickingOrderAsync(po));

            Assert.NotEmpty(await api.GetInProgressAssignedPickingOrdersAsync());
        }

        [Fact]
        public async Task TestProcessingPickingOrder() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            var pos = await api.GetOpenPickingOrdersAsync();
            Assert.NotNull(pos);
            Assert.NotEmpty(pos);

            var po = pos.First();
            Assert.NotNull(po);

            Assert.Empty(await api.GetFinishedPickingOrdersAsync());
            Assert.Empty(await api.GetInProgressAssignedPickingOrdersAsync());

            Assert.True(await api.AssignToPickingOrderAsync(po));

            Assert.NotEmpty(await api.GetInProgressAssignedPickingOrdersAsync());

            foreach (var pop in po.OrderPositions) {
                var spos = await api.GetStockPositionsForArticleAsync(pop.Article);
                Assert.NotNull(spos);
                Assert.NotEmpty(spos);

                var spo = spos.First();

                Assert.True(await api.PickAsync(pop, spo));
            }

            Assert.NotEmpty(await api.GetFinishedPickingOrdersAsync());
        }

        [Fact]
        public async Task TestFileDamageReport() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            var pos = await api.GetOpenPickingOrdersAsync();
            Assert.NotNull(pos);
            Assert.NotEmpty(pos);

            var po = pos.First();
            Assert.NotNull(po);

            var pop = po.OrderPositions.First();
            Assert.NotNull(pop);

            var nReports = (await api.GetArticleDamageReportsAsync()).Count();

            await api.ReportDamagedArticleAsync(pop.Article, "Hello there!");

            Assert.Equal(nReports + 1, (await api.GetArticleDamageReportsAsync()).Count());
        }

        /// <summary>
        /// Test for all employees, roles, if the authroization works.
        /// </summary>
        /// <param name="personnelNumber">The personnel number of the employee to test.</param>
        /// <returns>Returns a running task.</returns>
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(7)]
        public async Task TestAuthenticationAndAuthorization(byte personnelNumber) {
            KommissIOAPI api = new KommissIOAPI(uri);
            Employee? e = await api.IdentifyAndAuthenticateAysnc(personnelNumber, "password");
            Assert.NotNull(e);

            Role role = e.Role;

            if (!role.HasFlag(Role.Administrator)) {
                Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.ResetToDefaultAsync())).StatusCode);

                if (!role.HasFlag(Role.Manager)) {
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetArticleDamageReportsAsync())).StatusCode);
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetFinishedPickingOrdersAsync())).StatusCode);
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetInProgressPickingOrdersAsync())).StatusCode);
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetPickingOrdersAsync())).StatusCode);
                }

                if (!role.HasFlag(Role.Employee) && !role.HasFlag(Role.Manager)) {
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetStockPositionsForArticleAsync(new Article() { ArticleNumber = 1, Name = "" }))).StatusCode);
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetOpenPickingOrdersAsync())).StatusCode);
                }

                if (!role.HasFlag(Role.Employee)) {
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetInProgressAssignedPickingOrdersAsync())).StatusCode);
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.AssignToPickingOrderAsync(new PickingOrder() { Note = "", OrderPositions = new List<PickingOrderPosition>(), Priority = 1 }))).StatusCode);
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.ReportDamagedArticleAsync(new Article() { ArticleNumber = 1, Name = "" }, ""))).StatusCode);
                    Assert.Equal(System.Net.HttpStatusCode.Forbidden, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.PickAsync(new PickingOrderPosition() { Article = new Article() { ArticleNumber = 1, Name = "" }, DesiredAmount = 1, PickedAmount = 0 },
                    new StockPosition() { Amount = 10, Article = new Article() { ArticleNumber = 1, Name = "" }, ShelfNumber = 10 }))).StatusCode);
                }
            }
        }

        [Fact]
        public async Task TestWihtoutAuthenticationAuthorization() {
            KommissIOAPI api = new KommissIOAPI(uri);

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetInProgressAssignedPickingOrdersAsync())).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.ResetToDefaultAsync())).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetStockPositionsForArticleAsync(new Article() { ArticleNumber = 1, Name = "" }))).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.AssignToPickingOrderAsync(new PickingOrder() { Note = "", OrderPositions = new List<PickingOrderPosition>(), Priority = 1 }))).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetArticleDamageReportsAsync())).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetFinishedPickingOrdersAsync())).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetOpenPickingOrdersAsync())).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.PickAsync(new PickingOrderPosition() { Article = new Article() { ArticleNumber = 1, Name = "" }, DesiredAmount = 1, PickedAmount = 0 },
                new StockPosition() { Amount = 10, Article = new Article() { ArticleNumber = 1, Name = "" }, ShelfNumber = 10 }))).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.ReportDamagedArticleAsync(new Article() { ArticleNumber = 1, Name = "" }, ""))).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetInProgressPickingOrdersAsync())).StatusCode);
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, (await Assert.ThrowsAsync<HttpRequestException>(async () => await api.GetPickingOrdersAsync())).StatusCode);
        }

        [Fact]
        public async Task TestGetArticleByArticleNumber() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            var pos = await api.GetOpenPickingOrdersAsync();
            Assert.NotNull(pos);
            Assert.NotEmpty(pos);

            var po = pos.First();
            Assert.NotNull(po);

            var pop = po.OrderPositions.First();
            Assert.NotNull(pop);

            var article = await api.GetArticleByArticleNumberAsync(pop.Article.ArticleNumber);
            Assert.NotNull(article);
            Assert.Equal(pop.Article.ArticleNumber, article.ArticleNumber);
        }


        [Fact]
        public async Task TestGetStockPositionById() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            var pos = await api.GetOpenPickingOrdersAsync();
            Assert.NotNull(pos);
            Assert.NotEmpty(pos);

            var po = pos.First();
            Assert.NotNull(po);

            var pop = po.OrderPositions.First();
            Assert.NotNull(pop);

            var spos = await api.GetStockPositionsForArticleAsync(pop.Article);
            var spo = spos.First();

            var stockPos = await api.GetStockPositionByIdAsync(spo.Id);
            Assert.NotNull(stockPos);
            Assert.Equal(spo.Id, stockPos.Id);
        }

        [Fact]
        public async Task TestGetOrderById() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            var pos = await api.GetOpenPickingOrdersAsync();
            Assert.NotNull(pos);
            Assert.NotEmpty(pos);

            var po = pos.First();
            Assert.NotNull(po);

            var orderPOs = await api.GetPickingOrderByIdAsync(po.Id);
            Assert.NotNull(orderPOs);
            Assert.Equal(po.Id, orderPOs.Id);
        }

        [Fact]
        public async Task TestGetOrderPositionById() {
            IKommissIOAPI api = await SetupKommissIOAPI();

            var pos = await api.GetOpenPickingOrdersAsync();
            Assert.NotNull(pos);
            Assert.NotEmpty(pos);

            var po = pos.First();
            Assert.NotNull(po);

            var pop = po.OrderPositions.First();
            Assert.NotNull(pop);

            var orderPOs = await api.GetPickingOrderPositionByIdAsync(pop.Id);
            Assert.NotNull(orderPOs);
            Assert.Equal(pop.Id, orderPOs.Id);
        }
    }
}
