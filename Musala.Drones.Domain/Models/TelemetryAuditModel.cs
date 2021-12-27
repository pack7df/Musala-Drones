using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.Models
{
    public class TelemetryAuditModel
    {
        public DateTime Date;
        public TelemetryModel Value;
        public string serial;
    }
}
