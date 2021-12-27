using Musala.Drones.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.ServicesContracts
{
    public interface IDroneTelemetryService
    {
        TelemetryModel GetTelemetry(DroneModel drone);
    }
}
