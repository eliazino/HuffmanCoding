using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface IRouteRepository {
        Task<IEnumerable<T>> getClosestRoutes<T>(double longitude, double latitude,double maxDistanceKM);
    }
}
