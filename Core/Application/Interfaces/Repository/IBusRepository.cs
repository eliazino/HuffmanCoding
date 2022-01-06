using Core.Application.DTOs.Filter;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface IBusRepository {
        Task<IEnumerable<Bus>> getBus(BusFilter filter);
        Task<IEnumerable<Fares>> getBusFare(int routeID);
        Task<double> getBusMaxFare(int routeID);
    }
}
