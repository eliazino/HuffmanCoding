using AutoMapper;
using Core.Application.DTOs.Filter;
using Core.Application.Interfaces.Repository;
using Core.Models.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.MySQL;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.MySQL {
    [RegisterAsScoped]
    public class BusRepository : Repository, IRepository, IBusRepository {
        private readonly IDBCommand _DBcontext;
        private readonly IMapper _mapper;
        public BusRepository(IDBCommand IDBCommand, IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
        }

        public async Task<IEnumerable<Bus>> getBus(BusFilter filter) {
            string clause = "";
            List<object> param = new List<object>();
            string sql = "SELECT color, model, buses.busID, capacity, buses.id, plateNumber, buses.routeID, buses.issuerID, issuers.issuerName, routes.name AS routeName, buses.status, buses.note FROM buses, issuers, routes WHERE routes.id = buses.routeID AND buses.issuerID = issuers.issuerID";
            if(filter.fieldIsSet(nameof(filter.busID))) {
                clause += " AND (buses.busID = ? OR alias = ?)";
                param.Add(filter.busID);
                param.Add(filter.busID);
            }
            if (filter.fieldIsSet(nameof(filter.plateNumber))) {
                clause += " AND buses.plateNumber = ?";
                param.Add(filter.plateNumber);
            }
            if (filter.fieldIsSet(nameof(filter.issuerID))) {
                clause += " AND buses.issuerID = ?";
                param.Add(filter.issuerID);
            }
            return (await selectFromQuery<Bus>(sql+clause, param)).resultAsObject;
        }

        public async Task<IEnumerable<Fares>> getBusFare(int routeID) {
            string sql = "SELECT fares.id, fares.price, fares.pointA, fares.pointB, fares.routeID, stop_1.name as pointAName, stop_2.name AS pointBName FROM fares, stops stop_1, stops stop_2 WHERE fares.pointA = stop_1.id AND fares.pointB = stop_2.id AND fares.routeID = ?";
            return (await selectFromQuery<Fares>(sql, new List<object> { routeID })).resultAsObject;
        }

        public async Task<double> getBusMaxFare(int routeID) {
            string sql = "SELECT COALESCE(MAX(price), 0) AS maxFare FROM fares WHERE fares.routeID = ?";
            var maxList =  (List<MaxFare>)(await selectFromQuery<MaxFare>(sql, new List<object> { routeID })).resultAsObject;
            if(maxList != null) {
                return maxList[0].maxFare;
            }
            return 0;
        }

        
        public class MaxFare {
            public double maxFare { get; set; }
        }
    }
}
