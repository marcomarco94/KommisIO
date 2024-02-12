using DataRepoCore;
using Microsoft.AspNetCore.Mvc;

namespace DataRESTfulAPI.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class KommissIOAPIControler : ControllerBase {
        private readonly ILogger<KommissIOAPIControler> _logger;

        public KommissIOAPIControler(ILogger<KommissIOAPIControler> logger) {
            _logger = logger;
        }
    }
}
