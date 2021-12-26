using Musala.Drones.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.ServicesContracts
{
    public interface IDronesStorageServices
    {
        void SaveOrUpdate(DroneModel drone);
        DroneModel Load(string serial);
    }
}
