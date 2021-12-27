﻿using MongoDB.Driver;
using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
using System;
using System.Threading.Tasks;

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

        public async Task<DroneModel> LoadAsync(string serial)
        {
            var collection = client.GetDatabase(configuration.DatabaseName).GetCollection<DroneModel>("drones");
            return await collection.Find(d => d.Serial == serial).FirstOrDefaultAsync();
        }

        public async Task SaveOrUpdateAsync(DroneModel drone)
        {
            var collection = client.GetDatabase(configuration.DatabaseName).GetCollection<DroneModel>("drones");
            await collection.DeleteOneAsync(d => d.Serial == drone.Serial);
            await collection.InsertOneAsync(drone);
        }
    }
}
