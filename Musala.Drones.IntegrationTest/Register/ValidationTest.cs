using Musala.Drones.ApiHost;
using Musala.Drones.ApiHost.Helpers;
using Musala.Drones.Domain.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Musala.Drones.IntegrationTest.Register
{

    public class ValidationTest
    {
        private string droneUrl = "api/drone";

        public static IEnumerable<object[]> BadRequestRegistrationSamples()
        {
            yield return new DroneModel[]
            {
                null
            };
            yield return new DroneModel[]
            {
                new DroneModel
                {
                    Serial = null,
                    Weight = 100,
                },
            };
            yield return new DroneModel[]
            {
                new DroneModel
                {
                    Serial = "",
                    Weight = 100,
                },
            };
            yield return new DroneModel[]
            {
                new DroneModel
                {
                    Serial = "1234567890",
                    Weight = 0,
                },
            };
            yield return new DroneModel[]
            {
                new DroneModel
                {
                    Serial = "1234567890",
                    Weight = -1,
                }
            };

        }

        [Theory]
        [MemberData(nameof(BadRequestRegistrationSamples))]
        public void BadRequestValidationTest(DroneModel sample)
        {
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var result = client.HttpClient.PostAsync(droneUrl, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.BadRequest,result.StatusCode);
        }

        [Fact]
        public void SerialSizeValidationFailTest()
        {
            var serial = "";
            for (int i=0; i<101; i++)
            {
                serial += "0";
            }
            var sample = new DroneModel
            {
                Serial = serial,
                Weight = 100
            };
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var result = client.HttpClient.PostAsync(droneUrl, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            var response = result.Content.ReadAsStringAsync().Result;
            Assert.Contains("Serial number size exceed 100 chars".ToLower(), response.ToLower());
        }

        [Fact]
        public void WeigthValidationFailTest()
        {
            var sample = new DroneModel
            {
                Serial = "1234567890",
                Weight = 600
            };
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var result = client.HttpClient.PostAsync(droneUrl, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            var response = result.Content.ReadAsStringAsync().Result;
            Assert.Contains("Weight limit exceed 500".ToLower(), response.ToLower());
        }
    }
}
