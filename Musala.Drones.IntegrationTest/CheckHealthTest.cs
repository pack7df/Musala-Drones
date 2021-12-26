using Musala.Drones.ApiHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Musala.Drones.IntegrationTest
{
    public class CheckHealthTest
    {
        [Theory]
        [InlineData("/api/health/WebHost")]
        [InlineData("/api/health/database")]
        public void CheckSystemsService(string url)
        {
            var client = new ClientsFackade.DronesApiHostTestClient();
            client.ClearDb();
            client.Initialize<Startup>();
            var response = client.HttpClient.GetAsync(url).Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
