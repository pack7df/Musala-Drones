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
        private int telemetryPeriod;
        public DroneTelemetryServiceMock(int telemetryPeriod)
        {
            bateryConsumption[DroneStateEnum.Iddle] = 1.0d/(30d*60d*60d); //1 month
            bateryConsumption[DroneStateEnum.Loaded] = 1.0d/(2d*60d*60d); //2h
            bateryConsumption[DroneStateEnum.Delivered] = 1.0d / (2d * 60d * 60d); //2h
            bateryConsumption[DroneStateEnum.Delivering] = 1.0d / (2d * 60d * 60d); //2h
            bateryConsumption[DroneStateEnum.Loading] = 1.0d / (2d * 60d * 60d); //2h
            bateryConsumption[DroneStateEnum.Returning] = 1.0d / (4d * 60d * 60d); //4h
            this.telemetryPeriod = telemetryPeriod;
        }

        public TelemetryModel GetTelemetry(DroneModel drone)
        {
            var newLevel = drone.BateryLevel - bateryConsumption[drone.State]*telemetryPeriod/1000;
            var result = new TelemetryModel
            {
                BateryLevel = (float)newLevel
            };
            return result;
        }
    }
}
