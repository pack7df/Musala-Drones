using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musala.Drones.MongoInfrastructure
{
    public interface IDbCleaner
    {
        Task ClearAsync();
    }
}
