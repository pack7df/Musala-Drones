using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.Models
{
    public class MedicationModel
    {
        public string Name
        {
            get;set;
        }
        public float Weight
        {
            get;set;
        }
        public string Code
        {
            get;set;
        }
        public string Image64
        {
            get;set;
        }

        public bool IsValid
        {
            get
            {
                return true;
            }
        }
    }
}
