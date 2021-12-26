using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.ApiHost.Helpers
{
    public static class HttpClientExtensionss
    {
        public static StringContent GetStringContent(this object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
        }
    }
}
