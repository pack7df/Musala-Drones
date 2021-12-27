using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain
{
    public class DroneServices : IDroneServices
    {
        private IDronesStorageServices droneStorage;
        public DroneServices(IDronesStorageServices droneStorage)
        {
            this.droneStorage = droneStorage;
        }

        public async Task<DroneModel> LoadDroneAsync(string serial)
        {
            return await droneStorage.LoadAsync(serial);
        }

        public async Task<bool> RegisterAsync(DroneModel drone)
        {
            var current = await droneStorage.LoadAsync(drone.Serial);
            if (current != null)
                return false;
            drone.State = DroneStateEnum.Iddle;
            //TODO: Create a baterry simualtion.
            drone.BateryLevel = drone.BateryLevel;
            await droneStorage.SaveOrUpdateAsync(drone);
            return true;
        }
    }
}
