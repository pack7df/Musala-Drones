
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
        [HttpGet()]
        [Route("database")]
        public async Task<IActionResult> MongoDbStatus()
        {
            var check = await dbHealthCheck.GetStatusAsync();
            if (!string.IsNullOrEmpty(check))
                return Ok(check);
            return this.NoContent();
        }
    }
}
