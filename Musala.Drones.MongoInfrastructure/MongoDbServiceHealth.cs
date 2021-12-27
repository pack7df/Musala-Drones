using HealthChecks.MongoDb;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Musala.Drones.MongoInfrastructure
{
    public class MongoDbServiceHealth : IDbServiceHealth
    {
        public string Name => "MongoDB";

        private MongoDbHealthCheck mongoDbHealthCheck;
        public MongoDbServiceHealth(MongoDbConfiguration config)
        {
            var mongoUrl = MongoUrl.Create(config.ConnectionString);
            MongoClientSettings mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            var client = new MongoClient(mongoClientSettings);
            var db = client.GetDatabase(config.DatabaseName);
            var collection = db.GetCollection<DroneModel>("health");
            collection.InsertOne(new DroneModel());
            collection.DeleteMany(d => true);
            mongoDbHealthCheck = new MongoDbHealthCheck(config.ConnectionString, config.DatabaseName);
        }
        public async Task<string> GetStatusAsync()
        {
            try
            {
               HealthCheckResult result = await mongoDbHealthCheck.CheckHealthAsync(null, new CancellationTokenSource(1000).Token);

               return result.Status.ToString();
            }
            catch(Exception e)
            {
                return "";
            }
        }
    }
}
