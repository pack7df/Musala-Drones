using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.Domain.ServicesContracts
{
    public interface IDbServiceHealth
    {
        string Name
        {
            get;
        }
        Task<string> GetStatusAsync();
    }
}
