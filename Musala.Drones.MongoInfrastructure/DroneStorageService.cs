using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
using System;

namespace Musala.Drones.MongoInfrastructure
{
    public class DroneStorageService : IDronesStorageServices
    {
        public DroneModel Load(string serial)
        {
            throw new NotImplementedException();
        }

        public void SaveOrUpdate(DroneModel drone)
        {
            throw new NotImplementedException();
        }
    }
}
