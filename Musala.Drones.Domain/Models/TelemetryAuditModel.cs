using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.Models
{
    public class TelemetryAuditModel
    {
        public string Id
        {
            get;set;
        }
        public DateTime Date
        {
            get;set;
        }
        public TelemetryModel Value
        {
            get;set;
        }
        public string Serial
        {
            get;set;
        }
    }
}
