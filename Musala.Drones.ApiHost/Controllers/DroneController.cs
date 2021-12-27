using Microsoft.AspNetCore.Mvc;
using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
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
        private IDroneServices droneService;
        private IDronesStorageServices droneStorageService;
        public DroneController(IDroneServices droneServices, IDronesStorageServices droneStorageService)
        {
            this.droneService = droneServices;
            this.droneStorageService = droneStorageService;
        }
        [HttpPost]
        public async Task<IActionResult> RegisterDrone([FromBody] DroneModel data)
        {
            if (string.IsNullOrEmpty(data.Serial))
                return BadRequest();
            if (data.Weight <= 0)
                return BadRequest();
            if (data.Serial.Length > 100)
                return Ok("Serial number size exceed 100 chars");
            if (data.Weight > 500)
                return Ok("Weight limit exceed 500");
            if ((data.BateryLevel < 0) || (data.BateryLevel >100))
                return Ok("Batery level must be between 0 and 100");
            var result = await droneService.RegisterAsync(data);
            if (!result)
                return Ok("Serial exists");
            return Created("",data);
        }
        [HttpGet()]
        [Route("{serial}")]
        public async Task<DroneModel> GetDrone(string serial)
        {
            var drone = await this.droneStorageService.LoadAsync(serial);
            return drone;
        }
    }
}
