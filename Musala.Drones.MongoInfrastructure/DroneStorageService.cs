using MongoDB.Driver;
using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
using System;

namespace Musala.Drones.MongoInfrastructure
{
    public class DroneStorageService : IDronesStorageServices
    {
        private IMongoClient client;
        private MongoDbConfiguration configuration;
        public DroneStorageService(IMongoClient client, MongoDbConfiguration configuration)
        {
            this.client = client;
            this.configuration = configuration;
        }

        public DroneModel Load(string serial)
        {
            return null;
            //throw new NotImplementedException();
        }

        public void SaveOrUpdate(DroneModel drone)
        {
            throw new NotImplementedException();
        }
    }
}
