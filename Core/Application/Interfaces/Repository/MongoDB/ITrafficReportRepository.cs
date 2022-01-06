using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository.MongoDB {
    public interface ITrafficReportRepository {
        Task<bool> createReport(TrafficReport report);
        Task<IEnumerable<TrafficReport>> getReport(long startDate, long endDate);
    }
}
