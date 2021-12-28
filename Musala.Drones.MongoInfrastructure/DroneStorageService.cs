using MongoDB.Driver;
using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Musala.Drones.MongoInfrastructure
{
    public class DroneStorageService : IDronesStorageServices, IDbCleaner
    {
        private IMongoClient client;
        private MongoDbConfiguration configuration;
        public DroneStorageService(IMongoClient client, MongoDbConfiguration configuration)
        {
            this.client = client;
            this.configuration = configuration;
        }

        public async Task ClearAsync()
        {
            await client.DropDatabaseAsync(configuration.DatabaseName);
        }

        public async Task<List<DroneModel>> LoadAllAsync()
        {
            var collection = client.GetDatabase(configuration.DatabaseName).GetCollection<DroneModel>(nameof(DroneModel).ToLower());
            return await collection.Find(d => true).ToListAsync();
        }

        public async Task<DroneModel> LoadAsync(string serial)
        {
            var collection = client.GetDatabase(configuration.DatabaseName).GetCollection<DroneModel>(nameof(DroneModel).ToLower());
            return await collection.Find(d => d.Serial == serial).FirstOrDefaultAsync();
        }

        public async Task<List<DroneModel>> LoadAvailableAsync()
        {
            var collection = client.GetDatabase(configuration.DatabaseName).GetCollection<DroneModel>(nameof(DroneModel).ToLower());
            return await collection.Find(d => d.BateryLevel >= 25).ToListAsync();
        }

        public async Task SaveOrUpdateAsync(DroneModel drone)
        {
            var collection = client.GetDatabase(configuration.DatabaseName).GetCollection<DroneModel>(nameof(DroneModel).ToLower());
            await collection.DeleteOneAsync(d => d.Serial == drone.Serial);
            await collection.InsertOneAsync(drone);
        }

        public async Task SaveAsync(TelemetryAuditModel audit)
        {
            var collection = client.GetDatabase(configuration.DatabaseName).GetCollection<TelemetryAuditModel>(nameof(TelemetryAuditModel).ToLower());
            audit.Id = Guid.NewGuid().ToString();
            await collection.InsertOneAsync(audit);
        }

        public async Task<List<TelemetryAuditModel>> LoadAuditionsAsync(string serial)
        {
            var collection = client.GetDatabase(configuration.DatabaseName).GetCollection<TelemetryAuditModel>(nameof(TelemetryAuditModel).ToLower());
            return await collection.Find(d => d.Serial==serial).ToListAsync();
        }
    }
}
