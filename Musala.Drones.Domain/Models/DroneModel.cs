using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.Models
{
    public enum DroneTypeEnum
    {
        Light=0, 
        Middle=1,
        Cruiser=2,
        Heavy=3
    };

    public enum DroneStateEnum
    {
        Iddle = 0, 
        Loading = 1, 
        Loaded = 2, 
        Delivering = 3, 
        Delivered = 4, 
        Returning=5
    };

    public class DroneModel
    {
        public String Id
        {
            get;set;
        }
        public string Serial
        {
            get;set;
        }
        public DroneTypeEnum Type
        {
            get;set;
        }
        public double Weight
        {
            get;set;
        }
        public float BateryLevel
        {
            get;set;
        }
        public DroneStateEnum State
        {
            get;set;
        }

        public MedicationModel[] Payload
        {
            get;set;
        }
    }
}
