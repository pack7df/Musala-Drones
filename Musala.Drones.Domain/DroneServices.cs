using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Musala.Drones.Domain
{
    public class DroneServices : IDroneServices
    {
        private IDronesStorageServices droneStorage;

        private IDroneTelemetryService droneTelemetryService;


        private int telemetryPeriod = 10 * 1000;
        private void BateryUpdater()
        {
            while (true)
            {
                var drones = droneStorage.LoadAllAsync().Result;
                foreach(var d in drones)
                {
                    var telemetry = droneTelemetryService.GetTelemetry(d);
                    var audit = new TelemetryAuditModel
                    {
                        Date = DateTime.Now,
                        Serial = d.Serial,
                        Value = telemetry
                    };
                    droneStorage.SaveAsync(audit);
                    d.BateryLevel = audit.Value.BateryLevel;
                    droneStorage.SaveOrUpdateAsync(d);
                }
                Thread.Sleep(telemetryPeriod);
            }
        }

        public DroneServices(IDronesStorageServices droneStorage, IDroneTelemetryService droneTelemetryService, int telemetryPeriod=10*1000)
        {
            this.droneStorage = droneStorage;
            this.droneTelemetryService = droneTelemetryService;
            this.telemetryPeriod = telemetryPeriod;
            Task.Run(() => BateryUpdater());
        }

        public async Task<DroneModel> LoadDroneAsync(string serial)
        {
            return await droneStorage.LoadAsync(serial);
        }

        public async Task<DroneLoadResult> LoadPayloadAsync(string serial, MedicationModel[] medications)
        {
            var drone = await droneStorage.LoadAsync(serial);
            if (drone == null)
                return DroneLoadResult.NotFound;
            if (drone.BateryLevel < 25)
                return DroneLoadResult.BateryLow;
            var payloadWeight = medications.Sum(m => m.Weight);
            if (payloadWeight > drone.Weight)
                return DroneLoadResult.OverWeigth;
            drone.Payload = medications;
            drone.State = DroneStateEnum.Loading;
            await droneStorage.SaveOrUpdateAsync(drone);
            return DroneLoadResult.Ok;
        }

        public async Task<bool> RegisterAsync(DroneModel drone)
        {
            var current = await droneStorage.LoadAsync(drone.Serial);
            if (current != null)
                return false;
            drone.Id = Guid.NewGuid().ToString();
            drone.State = DroneStateEnum.Iddle;
            drone.Payload = new MedicationModel[0];
            //TODO: Create a batery simualtion.
            drone.BateryLevel = drone.BateryLevel;
            await droneStorage.SaveOrUpdateAsync(drone);
            return true;
        }
    }
}
