using AutoMapper;
using Core.Application.Interfaces.Repository;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.MongoDB;
using Infrastructure.Interfaces.MySQL;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.MySQL {
    [RegisterAsScoped]
    public class RouteRepository : Repository, IRepository, IRouteRepository {
        private readonly IDBCommand _DBcontext;
        private readonly IMapper _mapper;
        public RouteRepository(IDBCommand IDBCommand, IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
        }
        public async Task<IEnumerable<T>> getClosestRoutes<T>(double longitude, double latitude, double maxDistanceKM) {
            string sql = "SELECT `name`, lat, lgt, SQRT(POW(69.1 * (lat - ?), 2) + POW(69.1 * (?  - lgt) * COS(lat / 57.3), 2)) AS distance FROM stops HAVING distance < ? ORDER BY distance ASC LIMIT 5";
            return (await selectFromQuery<T>(sql, new List<object> { latitude, longitude, maxDistanceKM })).resultAsObject;
        }
    }
}
