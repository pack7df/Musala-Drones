
using Microsoft.AspNetCore.Mvc;
using Musala.Drones.Domain.ServicesContracts;
using System.Threading.Tasks;

namespace Musala.Drones.ApiHost.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private IDbServiceHealth dbHealthCheck;
        public HealthController(IDbServiceHealth dbHealthCheck)
        {
            this.dbHealthCheck = dbHealthCheck;
        }
        [Route("database")]
        public async Task<IActionResult> MongoDbStatus()
        {

            var check = await dbHealthCheck.GetStatusAsync();
            if (check)
                return Ok();
            return this.NoContent();
        }
    }
}
