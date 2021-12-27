using Musala.Drones.ApiHost;
using Musala.Drones.ApiHost.Helpers;
using Musala.Drones.Domain.Models;
using Newtonsoft.Json;
using Xunit;

namespace Musala.Drones.IntegrationTest.DroneLoad
{
    public class PersitencyTest
    {
        private string droneLoadUrl = "api/drone/{serial}";
        private string droneUrl = "api/drone";
        [Fact]
        public void EnsureFieldsAreStored()
        {
            var drone = new DroneModel
            {
                Serial = "1234567890",
                Weight = 400,
                Type = DroneTypeEnum.Heavy,
                BateryLevel = 45
            };
            var sample = new MedicationModel[] {
                new MedicationModel{
                    Code = "A45X__01",
                    Image64 = "0000000",
                    Name = "name-1_0",
                    Weight = 200,
                },
                new MedicationModel{
                    Code = "A45X__02",
                    Image64 = "0000000",
                    Name = "name-1_1",
                    Weight = 50,
                },
                new MedicationModel{
                    Code = "A45X__03",
                    Image64 = "0000000",
                    Name = "name-1_2",
                    Weight = 100,
                }
            };
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var _ = client.HttpClient.PostAsync(droneUrl, drone.GetStringContent()).Result;
            var url = droneLoadUrl.Replace("{serial}", drone.Serial);
            var result = client.HttpClient.PostAsync(url,sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            result = client.HttpClient.GetAsync($"{droneUrl}/{drone.Serial}").Result;
            var modelStr = result.Content.ReadAsStringAsync().Result;
            var returnedModel = JsonConvert.DeserializeObject<DroneModel>(modelStr);
            Assert.Equal(DroneStateEnum.Loading, returnedModel.State);
            Assert.Equal(sample.Length,returnedModel.Payload.Length);
            for (int i=0; i<returnedModel.Payload.Length; i++)
            {
                var p = returnedModel.Payload[i];
                var s = sample[i];
                Assert.Equal(s.Code,p.Code);
                Assert.Equal(s.Image64, p.Image64);
                Assert.Equal(s.Name, p.Name);
                Assert.Equal(s.Weight, p.Weight);
            }
        }
    }
}
