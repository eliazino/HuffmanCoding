using Core.Application.Interfaces.Repository.MongoDB;
using Core.Models.Entities;
using Infrastructure.Persistence;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.MongoDB {
    [RegisterAsScoped]
    public class TrafficReportRepository : Repository<TrafficReport>, ITrafficReportRepository, Interfaces.MongoDB.IRepository<TrafficReport> {
        public TrafficReportRepository(IMongoDb db) : base(db) { }

        public async Task<bool> createReport(TrafficReport report) {
            return await create(report);
        }

        public async Task<IEnumerable<TrafficReport>> getReport(long startDate, long endDate) {
            Expression<Func<TrafficReport, bool>> condition = f => f.dateCreated >= startDate & f.dateCreated <=endDate;
            return await getByCondition(condition);
        }
    }
}
