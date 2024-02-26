using AppDataAccessCore;
using DataRepoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendTests {
    public class KommissIOAPITests {
        readonly string uri = "https://kommissio.azurewebsites.net/api/";

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
    }
}
