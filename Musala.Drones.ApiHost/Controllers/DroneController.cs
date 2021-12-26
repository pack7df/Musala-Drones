using Microsoft.AspNetCore.Mvc;
using Musala.Drones.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Musala.Drones.ApiHost.Controllers
{
    [ApiController]
    [Route("api/drone")]
    public class DroneController : ControllerBase
    {
        [HttpPost]
        public IActionResult RegisterDrone([FromBody] DroneModel data)
        {
            if (string.IsNullOrEmpty(data.Serial))
                return BadRequest();
            if (data.Weight <= 0)
                return BadRequest();
            if (data.Serial.Length > 100)
                return Ok("Serial number size exceed 100 chars");
            if (data.Weight > 500)
                return Ok("Weight limit exceed 500");

            return null;
        }
    }
}
