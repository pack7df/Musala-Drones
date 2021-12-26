using HealthChecks.MongoDb;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Musala.Drones.Domain.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.MongoInfrastructure
{
    public class MongoDbServiceHealth : IDbServiceHealth
    {
        public string Name => "MongoDB";

        private MongoDbHealthCheck mongoDbHealthCheck;
        public MongoDbServiceHealth(string connectionString)
        {
            mongoDbHealthCheck = new MongoDbHealthCheck(connectionString);
        }
        public async Task<bool> GetStatusAsync()
        {
            HealthCheckResult result = await mongoDbHealthCheck.CheckHealthAsync(null);
            return result.Status == HealthStatus.Healthy;
        }
    }
}
