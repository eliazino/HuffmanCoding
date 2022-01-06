using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository.MongoDB {
    public interface IEscrowRepository {
        Task<bool> createEscrow(Transaction trx);
        Task<List<Transaction>> getEscrow(string transID);
        Task<bool> deleteEscrow(string transID);
    }
}
