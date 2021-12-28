using Musala.Drones.ApiHost;
using Musala.Drones.ApiHost.Helpers;
using Musala.Drones.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Musala.Drones.IntegrationTest.Telemetry
{
    public class TelemetryTest
    {
        private string droneUrl = "api/drone";
        private string droneAuditions = "api/drone/audit/{serial}";

        [Fact]
        public void IsCreatingAuditions()
        {

            var sample = new DroneModel
            {
                Serial = "1234567890",
                Weight = 250,
                Type = DroneTypeEnum.Heavy,
                BateryLevel = 45
            };
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var _ = client.HttpClient.PostAsync(droneUrl, sample.GetStringContent()).Result;
            Thread.Sleep(5500);
            var url = droneAuditions.Replace("{serial}", sample.Serial);
            var result = client.HttpClient.GetAsync(url).Result;
            var modelStr = result.Content.ReadAsStringAsync().Result;
            var returnedModels = JsonConvert.DeserializeObject<List<TelemetryAuditModel>>(modelStr);
            Assert.Equal(5,returnedModels.Count);
            Assert.True(returnedModels.All(m => m.Serial == sample.Serial));
        }

        [Fact]
        public void IsUpdatingBateryLevels()
        {
            var sample = new DroneModel
            {
                Serial = "1234567890",
                Weight = 250,
                Type = DroneTypeEnum.Heavy,
                BateryLevel = 45
            };
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var _ = client.HttpClient.PostAsync(droneUrl, sample.GetStringContent()).Result;
            Thread.Sleep(5500);
            var result = client.HttpClient.GetAsync($"{droneUrl}/{sample.Serial}").Result;
            var modelStr = result.Content.ReadAsStringAsync().Result;
            var returnedModels = JsonConvert.DeserializeObject<DroneModel>(modelStr);
            Assert.True(returnedModels.BateryLevel<sample.BateryLevel);
        }
    }
}
