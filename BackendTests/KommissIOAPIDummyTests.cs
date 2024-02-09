using DataRepoCore;

namespace BackendTests {
    public class KommissIOAPIDummyTests {
        [Fact]
        public async Task TestGetEmployeeAsync() {
            IKommissIOAPI rep = new KommissIOAPIDummy();
            //Trying to authenticate with different users.
            Assert.Null(await rep.IdentifyAndAuthenticateAysnc(0, "false"));
            Assert.Equal((short)1, (await rep.IdentifyAndAuthenticateAysnc(1, "admin"))?.PersonnelNumber);
            Assert.Equal((short)2, (await rep.IdentifyAndAuthenticateAysnc(2, "adminuser"))?.PersonnelNumber);
            Assert.Equal((short)3, (await rep.IdentifyAndAuthenticateAysnc(3, "user"))?.PersonnelNumber);
        }

        [Fact]
        public async Task TestGetOpenPositionAsync() {
            IKommissIOAPI rep = new KommissIOAPIDummy();

            //Check if auth is required.
            await Assert.ThrowsAsync<UnauthorizedAccessException>(rep.GetOpenPickingOrdersAsync);

            var authEmp = await rep.IdentifyAndAuthenticateAysnc(2, "adminuser");

            //Test if after auth. we can access the open orders.
            var pickingOrders = await rep.GetOpenPickingOrdersAsync();
            Assert.NotNull(pickingOrders);
            Assert.Equal(4, pickingOrders.Count());
        }

        [Fact]
        public async Task TestAssignEmployeeAsync() {
            IKommissIOAPI rep = new KommissIOAPIDummy();

            //Check if auth is required.
            await Assert.ThrowsAsync<UnauthorizedAccessException>(()=>rep.AssignEmployeeToPickingOrderAsync(new PickingOrder() { Id= 1, Note="", 
                OrderPositions=new List<PickingOrderPosition>(), Priority=1}));

            var authEmp = await rep.IdentifyAndAuthenticateAysnc(2, "adminuser");
            
            //Get the open picking-orders.
            var pickingOrders = await rep.GetOpenPickingOrdersAsync();

            //Assign an employee to an picking order.
            var selectedPickingOrder = pickingOrders.First();
            await rep.AssignEmployeeToPickingOrderAsync(selectedPickingOrder);

            //get the open picking orders after assinging one to an employee.
            pickingOrders = await rep.GetOpenPickingOrdersAsync();
            Assert.NotNull(pickingOrders);
            Assert.Equal(3, pickingOrders.Count());

            authEmp = await rep.IdentifyAndAuthenticateAysnc(2, "user");

            //Reassigning an open picking order is invalid.
            await Assert.ThrowsAsync<InvalidOperationException>(()=>rep.AssignEmployeeToPickingOrderAsync(selectedPickingOrder));
        }

        [Fact]
        public async Task TestPickingAsync() {
            IKommissIOAPI rep = new KommissIOAPIDummy();

            //Check if auth is required.
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => rep.GetStockPositionsForArticleAsync(new Article() { ArticleNumber=1, Name="A name"}));

            var authEmp = await rep.IdentifyAndAuthenticateAysnc(2, "adminuser");

            var pickingOrders = await rep.GetOpenPickingOrdersAsync();
            var pickingOrder = pickingOrders.First();
            await rep.AssignEmployeeToPickingOrderAsync(pickingOrder);

            var pop = pickingOrder.OrderPositions.First();
            var stockPosBefore = await rep.GetStockPositionsForArticleAsync(pop.Article);
            await rep.PickAsync(pop, stockPosBefore.First());

            //Trying to pick more items then stored will result in an error.
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => rep.PickAsync(pop, stockPosBefore.First(), 1000));

            //Validate that after picking n items the stock position has decreased
            var stockPosAfter = await rep.GetStockPositionsForArticleAsync(pop.Article);
            Assert.Equal(stockPosBefore.First().Amount - pop.DesiredAmount, stockPosAfter.First().Amount);
        }

        [Fact]
        public async Task ResetAsync() {
            IKommissIOAPI rep = new KommissIOAPIDummy();

            //Check if auth is required.
            await Assert.ThrowsAsync<UnauthorizedAccessException>(rep.ResetToDefaultAsync);

            var authEmp = await rep.IdentifyAndAuthenticateAysnc(2, "adminuser");

            Assert.True(await rep.ResetToDefaultAsync());

            //Pick something
            var pickingOrders = await rep.GetOpenPickingOrdersAsync();
            var pickingOrder = pickingOrders.First();
            await rep.AssignEmployeeToPickingOrderAsync(pickingOrder);

            var pop = pickingOrder.OrderPositions.First();
            var stockPosBefore = await rep.GetStockPositionsForArticleAsync(pop.Article);
            await rep.PickAsync(pop, stockPosBefore.First());

            Assert.True(await rep.ResetToDefaultAsync());

            var stockPosAfter = await rep.GetStockPositionsForArticleAsync(pop.Article);
            Assert.Equal(stockPosBefore.First().Amount, stockPosAfter.First().Amount);
        }

        [Fact]
        public async Task TestGetPickingPositionAsync() {
            IKommissIOAPI rep = new KommissIOAPIDummy();

            //Check if auth is required.
            await Assert.ThrowsAsync<UnauthorizedAccessException>(rep.GetOpenPickingOrdersAsync);

            var authEmp = await rep.IdentifyAndAuthenticateAysnc(2, "adminuser");

            //Test if after auth. we can access the open orders.
            var pickingOrders = await rep.GetPickingOrdersAsync();
            Assert.NotNull(pickingOrders);
            Assert.Equal(4, pickingOrders.Count());

            //Assign an employee to an picking order.
            var selectedPickingOrder = pickingOrders.First();
            await rep.AssignEmployeeToPickingOrderAsync(selectedPickingOrder);

            //get the open picking orders after assinging one to an employee.
            pickingOrders = await rep.GetPickingOrdersAsync();
            Assert.NotNull(pickingOrders);
            Assert.Equal(4, pickingOrders.Count());
        }

        [Fact]
        public async Task TestReportDamage() {
            IKommissIOAPI rep = new KommissIOAPIDummy();

            //Check if auth is required.
            await Assert.ThrowsAsync<UnauthorizedAccessException>(()=>rep.ReportDamagedArticleAsync(new DamageReport() { Article = new Article() { ArticleNumber=1, Name=""}, 
                Employee=new Employee() { FirstName = "", LastName = "", PersonnelNumber = 1, Role = Role.User }, Message="" }));

            var authEmp = await rep.IdentifyAndAuthenticateAysnc(2, "adminuser");

            Assert.Equal(2, (await rep.GetArticleDamageReportsAsync())?.Count());

            await rep.ReportDamagedArticleAsync(new DamageReport() {
                Article = new Article() { ArticleNumber = 1, Name = "" },
                Employee = new Employee() { FirstName = "", LastName = "", PersonnelNumber = 1, Role = Role.User },
                Message = ""
            });

            Assert.Equal(3, (await rep.GetArticleDamageReportsAsync())?.Count());
        }
    }
}