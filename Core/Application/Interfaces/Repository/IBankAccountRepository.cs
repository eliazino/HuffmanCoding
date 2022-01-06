using Core.Application.DTOs.Filter;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface IBankAccountRepository {
        Task<bool> createAccount(BankAccount account);
        Task<List<BankAccount>> getAccount(BankAccountFilter filter);
    }
}
