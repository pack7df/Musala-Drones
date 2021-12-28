using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.Models.DTO
{
    public class DroneRegistrationModel
    {
        public string Serial
        {
            get; set;
        }
        public DroneTypeEnum Type
        {
            get; set;
        }
        public double Weight
        {
            get; set;
        }
        public float BateryLevel
        {
            get; set;
        }
    }
}
