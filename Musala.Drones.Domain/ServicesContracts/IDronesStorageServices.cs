﻿using Musala.Drones.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.ServicesContracts
{
    public interface IDronesStorageServices
    {
        Task SaveOrUpdateAsync(DroneModel drone);
        Task<DroneModel> LoadAsync(string serial);

        Task<List<DroneModel>> LoadAvailableAsync();
    }
}
