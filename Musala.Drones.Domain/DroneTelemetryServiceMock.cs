using Musala.Drones.Domain.Models;
using Musala.Drones.Domain.ServicesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain
{
    public class DroneTelemetryServiceMock : IDroneTelemetryService
    {
        private Dictionary<DroneStateEnum, double> bateryConsumption = new Dictionary<DroneStateEnum, double>();
        public DroneTelemetryServiceMock()
        {
            bateryConsumption[DroneStateEnum.Iddle] = 1.0d/(356d*24d*60d*60d); //1 year
            bateryConsumption[DroneStateEnum.Loaded] = 1.0d/(2d*60d*60d); //2h
            bateryConsumption[DroneStateEnum.Delivered] = 1.0d / (2d * 60d * 60d); //2h
            bateryConsumption[DroneStateEnum.Delivering] = 1.0d / (2d * 60d * 60d); //2h
            bateryConsumption[DroneStateEnum.Loading] = 1.0d / (2d * 60d * 60d); //2h
            bateryConsumption[DroneStateEnum.Returning] = 1.0d / (4d * 60d * 60d); //4h
        }

        public TelemetryModel GetTelemetry(DroneModel drone)
        {
            throw new NotImplementedException();
        }
    }
}
