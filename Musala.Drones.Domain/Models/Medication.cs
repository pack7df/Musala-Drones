using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
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

        [JsonIgnore]
        public bool IsValid
        {
            get
            {
                var namePattern = "^[A-Za-z0-9-_]+$";
                var nameMatch = Regex.IsMatch(Name,namePattern);
                if (!nameMatch) return false;
                var codePattern = "^[A-Z0-9_]+$";
                var codeMatch = Regex.IsMatch(Code, codePattern);
                if (!codeMatch) return false;
                return true;
            }
        }
    }
}
