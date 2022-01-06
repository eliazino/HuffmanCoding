using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface IDayPassRepository {
        Task<DayPass> getPassTransaction(string transID);
        Task<bool> savePassTransaction(string transID, DayPass pass, int expiry);
        Task<DayPass> fetchPass(long id);
        Task<List<DayPass>> fetchPass();
    }
}
