using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.Models.DTO;
using Musala.Drones.Domain.ServicesContracts;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Musala.Drones.ApiHost.Controllers
{
    [ApiController]
    [Route("api/drone")]
    [Produces("application/json")]
    public class DroneController : ControllerBase
    {
        private IDroneServices droneService;
        private IDronesStorageServices droneStorageService;
        public DroneController(IDroneServices droneServices, IDronesStorageServices droneStorageService)
        {
            this.droneService = droneServices;
            this.droneStorageService = droneStorageService;
        }
        /// <summary>
        /// Register a drone
        /// </summary>
        /// <param name="data">
        ///     Data for drone registration.
        /// </param>
        /// <remarks>
        /// Sample request:
        ///     POST /api/drone
        ///     {
        ///        "Serial": "serial_0",
        ///        "Type": 2,
        ///        "Weight": 250,
        ///        "BateryLevel" : 45
        ///     }
        /// </remarks>
        /// <returns>The drone data registered or an error message.</returns>
        /// <response code="201">Returns the newly created item.</response>
        /// <response code="400">If there is a validation problem.</response>
        [HttpPost]
        [ProducesResponseType(typeof(DroneModel),StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string),StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] DroneRegistrationModel data)
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
            var drone = new DroneModel
            {
                BateryLevel = data.BateryLevel,
                Payload = new MedicationModel[0],
                Serial = data.Serial,
                State = DroneStateEnum.Iddle,
                Type = data.Type,
                Weight = data.Weight
            };
            var result = await droneService.RegisterAsync(drone);
            if (!result)
                return BadRequest("Serial exists");
            return Created($"/api/drone/{data.Serial}", drone);
        }

        /// <summary>
        /// Get all available drones wich can carry a payload.
        /// </summary>
        /// <returns>The drone list.</returns>
        [HttpGet()]
        [Route("available")]
        public async Task<List<DroneModel>> Get()
        {
            return await this.droneStorageService.LoadAvailableAsync();
        }

        /// <summary>
        /// Get all available drones.
        /// </summary>
        /// <returns>The drone list.</returns>
        [HttpGet()]
        public async Task<List<DroneModel>> GetAll()
        {
            return await this.droneStorageService.LoadAllAsync();
        }

        /// <summary>
        /// Get a drone with the given serial.
        /// </summary>
        /// <param name="serial">Serial codel. </param>
        /// <returns>A drone with the specified serial.</returns>
        [HttpGet("{serial}")]
        public async Task<DroneModel> Get(string serial)
        {
            var drone = await this.droneStorageService.LoadAsync(serial);
            return drone;
        }

        /// <summary>
        /// Get a list of auditions of a drone.
        /// </summary>
        /// <param name="serial">Drone serial to find auditions</param>
        /// <returns>A drone with the specified serial.</returns>
        [HttpGet("audit/{serial}")]
        public async Task<List<TelemetryAuditModel>> GetAuditions(string serial)
        {
            var auditions = await this.droneStorageService.LoadAuditionsAsync(serial);
            return auditions;
        }

        /// <summary>
        /// Load a payload to a drone.
        /// </summary>
        /// <param name="serial">Serial code of a drone.</param>
        /// <param name="data">
        ///     Medications list.
        /// </param>
        /// <returns>Returns nothing if it wass success or a message with the error.</returns>
        /// <response code="200">The operation was success.</response>
        /// <response code="400">If there is a validation problem.</response>
        [HttpPost]
        [Route("{serial}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Clear databse and create samples.
        /// </summary>
        /// <response code="200">The operation was success.</response>
        [HttpPost]
        [Route("seed")]
        public async Task<IActionResult> GenerateSamples()
        {
            await droneStorageService.ResetDatabaseAsync();
            return Ok();
        }
    }
}
