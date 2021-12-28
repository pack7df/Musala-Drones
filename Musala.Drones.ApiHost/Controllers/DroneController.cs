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
        public async Task<IActionResult> Register([FromBody] DroneModel data)
        {
            if (string.IsNullOrEmpty(data.Serial))
                return BadRequest();
            if (data.Weight <= 0)
                return BadRequest();
            if (data.Serial.Length > 100)
                return BadRequest("Serial number size exceed 100 chars");
            if (data.Weight > 500)
                return BadRequest("Weight limit exceed 500");
            if ((data.BateryLevel < 0) || (data.BateryLevel > 100))
                return BadRequest("Batery level must be between 0 and 100");
            var result = await droneService.RegisterAsync(data);
            if (!result)
                return BadRequest("Serial exists");
            return Created($"/api/drone/{data.Serial}", data);
        }

        [HttpGet()]
        [Route("available")]
        public async Task<List<DroneModel>> Get()
        {
            return await this.droneStorageService.LoadAvailableAsync();
        }

        [HttpGet("{serial}")]
        public async Task<DroneModel> Get(string serial)
        {
            var drone = await this.droneStorageService.LoadAsync(serial);
            return drone;
        }

        [HttpGet("audit/{serial}")]
        public async Task<List<TelemetryAuditModel>> GetAuditions(string serial)
        {
            var auditions = await this.droneStorageService.LoadAuditionsAsync(serial);
            return auditions;
        }

        [HttpPost]
        [Route("{serial}")]
        public async Task<IActionResult> LoadPayload(string serial, [FromBody] MedicationModel[] data)
        {
            if (data.Length==0)
                return BadRequest();
            if (data.Any(m => m == null))
                return BadRequest();
            var names = data.Select(m => m.Name).ToHashSet();
            var codes = data.Select(m => m.Code).ToHashSet();
            if (names.Count() != data.Length)
                return BadRequest("Repeated items");
            if (codes.Count() != data.Length)
                return BadRequest("Repeated items");
            if (data.Any(m => !m.IsValid))
                return BadRequest("Invalid code or name in medication.");
            var result = await droneService.LoadPayloadAsync(serial, data);
            switch (result)
            {
                case DroneLoadResult.BateryLow:
                {
                    return BadRequest("Batery low");
                }
                case DroneLoadResult.OverWeigth:
                {
                    return BadRequest("OverWeight");
                }
                case DroneLoadResult.NotFound:
                {
                    return BadRequest("Drone not found");
                }
                case DroneLoadResult.Ok:
                {
                    return Ok();
                }
            }
            return Ok();
        }
    }
}
