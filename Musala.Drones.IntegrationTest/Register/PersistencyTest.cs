using Musala.Drones.ApiHost;
using Musala.Drones.ApiHost.Helpers;
using Musala.Drones.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Musala.Drones.IntegrationTest.Register
{
    public class PersistencyTest
    {
        private string droneUrl = "api/drone";

        [Fact]
        public void EnsureFieldsAreReturned()
        {
            var sample = new DroneModel
            {
                Serial = "1234567890",
                Weight = 250,
                Type = DroneTypeEnum.Heavy,
            };
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var result = client.HttpClient.PostAsync(droneUrl, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.Created, result.StatusCode);
            var modelStr = result.Content.ReadAsStringAsync().Result;
            var returnedModel = JsonConvert.DeserializeObject<DroneModel>(modelStr);
            Assert.Empty(returnedModel.Payload);
            Assert.Equal(sample.Serial, returnedModel.Serial);
            Assert.Equal(DroneStateEnum.Iddle, returnedModel.State);
            Assert.Equal(sample.Type, returnedModel.Type);
            Assert.Equal(sample.Weight, returnedModel.Weight);
        }

        [Fact]
        public void EnsureFieldsAreStored()
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
            var result = client.HttpClient.GetAsync($"{droneUrl}/{sample.Serial}").Result;
            var modelStr = result.Content.ReadAsStringAsync().Result;
            var returnedModel = JsonConvert.DeserializeObject<DroneModel>(modelStr);
            Assert.Empty(returnedModel.Payload);
            Assert.Equal(sample.Serial, returnedModel.Serial);
            Assert.Equal(DroneStateEnum.Iddle, returnedModel.State);
            Assert.Equal(sample.Type, returnedModel.Type);
            Assert.Equal(sample.Weight, returnedModel.Weight);
            Assert.Equal(sample.BateryLevel, returnedModel.BateryLevel);
            Assert.Empty(returnedModel.Payload);
        }
    }
}
