using DataRepoCore;

namespace BackendTests {
    public class KommissIOAPIDummyTests {
        [Fact]
        public async Task TestGetEmployee() {
            IKommissIOAPI rep = new KommissIOAPIDummy();
            Assert.Null(await rep.IdentifyAndAuthenticateAysnc(0, "false"));
            Assert.Equal((short)0, (await rep.IdentifyAndAuthenticateAysnc(0, "admin"))?.PersonnelNumber);
            Assert.Equal((short)1, (await rep.IdentifyAndAuthenticateAysnc(1, "adminuser"))?.PersonnelNumber);
            Assert.Equal((short)2, (await rep.IdentifyAndAuthenticateAysnc(2, "user"))?.PersonnelNumber);
        }
    }
}