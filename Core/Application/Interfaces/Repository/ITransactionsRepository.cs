using Core.Application.DTOs.Local;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface ITransactionsRepository {
        Task<bool> walletTransfer(WalletTransfer transferData);
        Task<bool> walletPayment(WalletTransfer transferData, bool debitWallet = true);
        Task<bool> refundTicket(string transID, string phone, string creditor, double amount);
        Task<bool> creditWallet(WalletTopupDTO topupData);
        Task<bool> walletCredited(string transID);
    }
}
