using Core.Application.DTOs.Filter;
using Core.Application.DTOs.Local;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface ICustomerRepository {
        Task<IEnumerable<Customer>> get(CustomerFilter filter);
        Task<IEnumerable<T>> get<T>(CustomerFilter filter) where T : class;
        Task<bool> customerExists(CustomerFilter filter);
        Task<bool> create(Customer customer);
        Task<bool> updateSelf(Customer customer);
        Task<bool> updateProfileImage(string username, string path);
        Task<bool> updatePIN(string pin, string username);
        Task<bool> updatePassword(string username, string password);
        Task<double> getBalance(string username);        
        Task<IEnumerable<CardBalance>> getAccounts(string phone);
        Task<bool> updateSession(string publicKey, long lastseen, string username);
        Task<bool> updateBankAccount(string username, string account);
        Task<bool> debitWallet(double amount, string username);
        Task<bool> firstUpdate(Customer customer);
    }
}
