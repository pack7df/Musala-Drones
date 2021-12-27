using Musala.Drones.ApiHost;
using Musala.Drones.ApiHost.Helpers;
using Musala.Drones.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Musala.Drones.IntegrationTest.DroneLoad
{
    public class ValidationTest
    {
        private string droneLoadUrl = "api/drone/{serial}/load";
        private string droneRegisterUrl = "api/drone/{serial}/load";

        public static IEnumerable<object[]> BadRequestSamples()
        {
            //Medication list is null
            yield return new object[]
            {
                "1234567890",
                null
            };
            //Medication list is empty
            yield return new object[]
            {
                "1234567890",
                new MedicationModel[0]
            };
            //Medication list has some null value
            yield return new object[]
            {
                "1234567890",
                new MedicationModel[]
                {
                    new MedicationModel
                    {
                        Code = "A45X__01",
                        Image64 = "0000000000",
                        Name = "name-1_0",
                        Weight = 250
                    },
                    null,
                    new MedicationModel
                    {
                        Code = "A45X__02",
                        Image64 = "0000000000",
                        Name = "name-1_1",
                        Weight = 252
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(BadRequestSamples))]
        public void BadRequestValidationTest(string serial, MedicationModel[] sample)
        {
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var url = droneLoadUrl.Replace("{serial}", serial);
            var result = client.HttpClient.PostAsync(url, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        }

        public static IEnumerable<object[]> RepeatedItemsSamples()
        {
            //Medication list has some repeated name
            yield return new object[]
            {
                "1234567890",
                new MedicationModel[]
                {
                    new MedicationModel
                    {
                        Code = "A45X__01",
                        Image64 = "0000000000",
                        Name = "name-1_0",
                        Weight = 250
                    },
                    new MedicationModel
                    {
                        Code = "A45X__02",
                        Image64 = "0000000000",
                        Name = "name-1_1",
                        Weight = 250
                    },
                    new MedicationModel
                    {
                        Code = "A45X__03",
                        Image64 = "0000000000",
                        Name = "name-1_0",
                        Weight = 255
                    }
                }
            };
            //Medication list has some repeated code
            yield return new object[]
            {
                "1234567890",
                new MedicationModel[]
                {
                    new MedicationModel
                    {
                        Code = "A45X__01",
                        Image64 = "0000000000",
                        Name = "name-1_0",
                        Weight = 250
                    },
                    new MedicationModel
                    {
                        Code = "A45X__02",
                        Image64 = "0000000000",
                        Name = "name-1_1",
                        Weight = 250
                    },
                    new MedicationModel
                    {
                        Code = "A45X__01",
                        Image64 = "0000000000",
                        Name = "name-1_2",
                        Weight = 255
                    }
                }
            };
        }

        [Theory]
        [MemberData(nameof(RepeatedItemsSamples))]
        public void RepeatedItemsValidationTest(string serial, MedicationModel[] sample)
        {
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var url = droneLoadUrl.Replace("{serial}", serial);
            var result = client.HttpClient.PostAsync(url, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.Conflict, result.StatusCode);
        }



        public static IEnumerable<object[]> OverWeightItemsSamples()
        {
            //Medication list more mass than can lift the drone
            yield return new object[]
            {
                (float)400,
                "1234567890",
                new MedicationModel[]
                {
                    new MedicationModel
                    {
                        Code = "A45X__01",
                        Image64 = "0000000000",
                        Name = "name-1_0",
                        Weight = 250
                    },
                    new MedicationModel
                    {
                        Code = "A45X__02",
                        Image64 = "0000000000",
                        Name = "name-1_1",
                        Weight = 100
                    },
                    new MedicationModel
                    {
                        Code = "A45X__03",
                        Image64 = "0000000000",
                        Name = "name-1_2",
                        Weight = 100
                    }
                }
            };

        }

        [Theory]
        [MemberData(nameof(OverWeightItemsSamples))]
        public void OverWeightItemsValidationTest(float maxWeight, string serial, MedicationModel[] sample)
        {
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var drone = new DroneModel
            {
                Serial = serial,
                Type = DroneTypeEnum.Heavy,
                Weight = maxWeight
            };
            var _ = client.HttpClient.PostAsync(droneRegisterUrl, drone.GetStringContent()).Result;
            var url = droneLoadUrl.Replace("{serial}", serial);
            var result = client.HttpClient.PostAsync(url, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.Conflict, result.StatusCode);
        }

        [Fact]
        public void BateryBelow25ValidationTest()
        {
            var maxWeight = 400;
            var serial = "01234567890";
            var sample = new MedicationModel[] {
                new MedicationModel{
                    Code = "A45X__02",
                    Image64 = "0000000",
                    Name = "name-1_0",
                    Weight = 200,
                }
            };
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var drone = new DroneModel
            {
                Serial = serial,
                Type = DroneTypeEnum.Heavy,
                Weight = maxWeight,
                BateryLevel = 20
            };
            var _ = client.HttpClient.PostAsync(droneRegisterUrl, drone.GetStringContent()).Result;
            var url = droneLoadUrl.Replace("{serial}", serial);
            var result = client.HttpClient.PostAsync(url, sample.GetStringContent()).Result;
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            var content = result.Content.ReadAsStringAsync().Result;
            Assert.Equal("Batery low".ToLower(), content.ToLower());
        }
    }
}
