using DataRepoCore;

namespace BackendTests {
    public class EmployeeRepositroyDummyTests {
        [Fact]
        public void TestInvalidOperationException() {
            IEmployeeRepository rep = new EmployeeRepositoryDummy();
            Assert.ThrowsAsync<InvalidOperationException>(async () => { await rep.InsertAsync(new Employee()); });
            Assert.ThrowsAsync<InvalidOperationException>(async () => { await rep.UpdateAsync(new Employee()); });
            Assert.ThrowsAsync<InvalidOperationException>(async () => { await rep.DeleteAsync(0); });
            Assert.ThrowsAsync<InvalidOperationException>(async () => { await rep.GetElementByIDAsync(0); });
            Assert.ThrowsAsync<InvalidOperationException>(async () => { await rep.GetElementsAsync(); });
        }

        [Fact]
        public async Task TestGetEmployee() {
            IEmployeeRepository rep = new EmployeeRepositoryDummy();
            Assert.Null(await rep.IdentifyAndAuthenticateAysnc(0, "false"));
            Assert.Equal((short)0, (await rep.IdentifyAndAuthenticateAysnc(0, "admin"))?.PersonnelNumber);
            Assert.Equal((short)1, (await rep.IdentifyAndAuthenticateAysnc(1, "adminuser"))?.PersonnelNumber);
            Assert.Equal((short)2, (await rep.IdentifyAndAuthenticateAysnc(2, "user"))?.PersonnelNumber);
        }
    }
}