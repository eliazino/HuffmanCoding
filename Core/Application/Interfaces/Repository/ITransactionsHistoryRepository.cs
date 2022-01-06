using Core.Application.DTOs.Local;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository {
    public interface ITransactionsHistoryRepository {
        Task<IEnumerable<CardTransactions>> getCardTransactions(string cardSerial, long dateFrom, long dateTo);
        Task<IEnumerable<BusTransactions>> getBusHistory(string cardSerial, long dateFrom, long dateTo, bool isWalletPayment = false);
        Task<IEnumerable<TransferTransactions>> getTransfers(string phone, long dateFrom, long dateTo);
        Task<IEnumerable<WalletTopupTransactions>> getWalletTopups(string username, long dateFrom, long dateTo);
        Task<IEnumerable<WalletTransfer>> getTickets(string phone, string transID = null);
        Task<DashboardData> getDashboard(string cardSerial, long dateFrom, long dateTo);
    }
}
