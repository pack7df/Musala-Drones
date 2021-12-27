using Musala.Drones.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.ServicesContracts
{
    public enum DroneLoadResult
    {
        Ok,BateryLow=1,OverWeigth=2,RepeatedItems=3,NotFound=4
    }
    public interface IDroneServices
    {
        public Task<bool> RegisterAsync(DroneModel drone);
        public Task<DroneLoadResult> LoadPayloadAsync(string serial, MedicationModel[] medications);
    }
}
