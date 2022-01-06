using Core.Application.DTOs.Filter;
using Core.Application.DTOs.Request;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface IMerchantPayRepository {
        Task<bool> completePayment(MerchantPay transactions, CustomerDTO identity);
        Task<bool> escrowPayment(MerchantPay transactions, CustomerDTO identity);
        Task<List<Merchant>> getMerchant(MerchantFilter Filter);
        Task<List<Department>> getDepartment(DepartmentFilter Filter);
        Task<List<Payment>> getProduct(ProductFilter Filter);
        Task<List<Transfer>> getMerchantPayLog(MerchantPayTransferFilter filter);
        Task<bool> merchantLogExists(MerchantPayTransferFilter filter);
        Task<List<Transaction>> getMerchantPayTransactions(MerchantPayTransactionFilter filter);
    }
}
