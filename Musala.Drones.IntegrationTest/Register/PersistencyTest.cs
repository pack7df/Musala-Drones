using Musala.Drones.ApiHost;
using Musala.Drones.ApiHost.Helpers;
using Musala.Drones.Domain.Models;
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
        private string registerDroneUrl = "api/drone";


        [Fact]
        public void EnsureFieldsAreStored()
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
            var result = client.HttpClient.PostAsync(registerDroneUrl, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.Created, result.StatusCode);
            //TODO, Ensure data is stored.
        }
    }
}
