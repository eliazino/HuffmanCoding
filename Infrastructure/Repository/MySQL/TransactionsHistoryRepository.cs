using AutoMapper;
using Core.Application.DTOs.Configurations;
using Core.Application.DTOs.Local;
using Core.Application.Interfaces.Repository;
using Core.Models.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.MySQL;
using Microsoft.Extensions.Options;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.MySQL {
    [RegisterAsScoped]
    public class TransactionsHistoryRepository : Repository, ITransactionsHistoryRepository, IRepository {
        private readonly IDBCommand _DBcontext;
        private readonly IMapper _mapper;
        private readonly AppConfig _config;
        private readonly MPayConfig _busPayConfig;
        public TransactionsHistoryRepository(IDBCommand IDBCommand, IMapper _mapper, IOptionsMonitor<SystemVariables> config) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
            _config = config.CurrentValue.AppConfig;
            _busPayConfig = config.CurrentValue.BusPaymentConfig;
        }
        public async Task<IEnumerable<BusTransactions>> getBusHistory(string cardSerial, long dateFrom, long dateTo, bool isWalletPayment = false) {
            string typeComparer = isWalletPayment ? " LIKE " : " NOT LIKE ";
            string sql = string.Concat("SELECT stops.name as exitPoint, tmp_2.* FROM (SELECT stops.name as entryPoint, tmp_1.* FROM (SELECT bustransactions.amount, bustransactions.checkout, bustransactions.cardSerial, bustransactions.transDate, bustransactions.id, bustransactions.pointA, bustransactions.pointB, routes.name AS routeName FROM bustransactions LEFT JOIN routes ON routes.id = bustransactions.itemID WHERE cardSerial = ? AND transDate BETWEEN ? AND ? AND transID ", typeComparer, " '_walletPay%' LIMIT ", _config.maxGetRows,") AS tmp_1 LEFT JOIN stops ON stops.id = tmp_1.pointA) AS tmp_2 LEFT JOIN stops ON stops.id = tmp_2.pointB ORDER BY tmp_2.transDate DESC");
            var data = await selectFromQuery<BusTransactions>(sql, new List<object> { cardSerial, dateFrom, dateTo });
            return data.resultAsObject;
        }        

        public async Task<IEnumerable<CardTransactions>> getCardTransactions(string cardSerial, long dateFrom, long dateTo) {
            string sql = "SELECT id, amount, transDate, IF(transType IN (1, 3), 'Cr', 'Dr') as entry, cardSerial, transType, cardBalance FROM transactions WHERE cardSerial = ? AND transDate BETWEEN ? AND ?" +
              " UNION SELECT id, amount, transDate, 'Cr' as entry, cardSerial, 100 AS transType, finalCardBal AS cardBalance FROM transfers WHERE source = 'wallet' AND cardSerial = ? AND transDate BETWEEN ? AND ? " +
              " UNION SELECT id, amount, transDate, 'Dr' AS entry, cardSerial, IF(issuerID = 'CARDSWITCH', 102, 101) as transType, finalCardBal AS cardBalance FROM transfers WHERE source = 'card' AND cardSerial = ? AND transDate BETWEEN ? AND ?" +
              " UNION SELECT id, amount, transDate, 'Dr' as entry, cardSerial, 103 AS transType, (cardBalance + famount - amount) AS cardBalance FROM bustransactions WHERE cardSerial = ? AND transDate BETWEEN ? AND ? AND transID NOT LIKE '_walletPay%'" +
              " UNION SELECT id, amount, transDate, 'Dr' as entry, cardSerial, -103 AS transType, (cardBalance + famount - amount) AS cardBalance FROM dupbustrans WHERE cardSerial = ? AND transDate BETWEEN ? AND ? AND transID NOT LIKE '_walletPay%'";
            sql = string.Concat("SELECT tmp.* FROM (", sql, ") AS tmp ORDER BY tmp.transDate DESC LIMIT ", _config.maxGetRows);
            var data = await selectFromQuery<CardTransactions>(sql, new List<object> { cardSerial, dateFrom, dateTo,
                cardSerial, dateFrom, dateTo, cardSerial, dateFrom, dateTo, cardSerial, dateFrom, dateTo, cardSerial, dateFrom, dateTo });
            return data.resultAsObject;
        }

        public async Task<DashboardData> getDashboard(string cardSerial, long dateFrom, long dateTo) {
            string totalBustransactions = "SELECT COALESCE(SUM(amount), 0) AS totalBusTransactions, COALESCE(COUNT(id),0) AS totalBusCount, COALESCE(SUM(IF(checkout > 1, checkout - transDate, 0)),0) AS averageTimeSpentInTransit, COALESCE(COUNT(IF(checkout > 2, 1, null)),0) AS checkouts FROM bustransactions WHERE cardSerial = ? AND transDate BETWEEN ? AND ?";
            var data = (List<DashboardData>)(await selectFromQuery<DashboardData>(totalBustransactions, new List<object>{ cardSerial, dateFrom, dateTo })).resultAsObject;
            var dashboard = data[0];
            dashboard.averageTimeSpentInTransit = ((dashboard.averageTimeSpentInTransit / dashboard.checkouts)/60);
            dashboard.checkoutRate = (float)(dashboard.totalBusCount / dashboard.checkouts);
            dashboard.averageFare = (dashboard.totalBusTransactions / dashboard.totalBusCount);
            return dashboard;
        }

        public async Task<IEnumerable<WalletTransfer>> getTickets(string phone, string transID = null) {
            string sql = "SELECT transfers.*, ? AS baseAccount FROM transfers WHERE phone = ? AND issuerID = ? ORDER BY id DESC";
            var param = new List<object> { phone, phone, _busPayConfig.beneficiaryIssuer };
            if (transID != null) {
                sql = "SELECT transfers.*, ? AS baseAccount FROM transfers WHERE phone = ? AND issuerID = ? AND transID = ? ORDER BY id DESC";
                param.Add(transID);
            }
            var data = await selectFromQuery<WalletTransfer>(sql, param);
            return data.resultAsObject;
        }

        public async Task<IEnumerable<TransferTransactions>> getTransfers(string phone, long dateFrom, long dateTo) {
            string sql = "SELECT transfers.*, ? AS baseAccount FROM transfers WHERE transDate BETWEEN ? AND ? AND ? IN (phone, beneficiary) ORDER BY id DESC LIMIT "+_config.maxGetRows;
            var data = await selectFromQuery<TransferTransactions>(sql, new List<object> { phone, dateFrom, dateTo, phone });
            return data.resultAsObject;
        }

        public async Task<IEnumerable<WalletTopupTransactions>> getWalletTopups(string username, long dateFrom, long dateTo) {
            string sql = "SELECT id, amount, transDate, transID, 'Cr' AS entry FROM topup WHERE cashierID = ? AND transDate BETWEEN ? AND ? ORDER BY id DESC LIMIT "+_config.maxGetRows;
            var data = await selectFromQuery<WalletTopupTransactions>(sql, new List<object> { username, dateFrom, dateTo });
            return data.resultAsObject;
        }
    }
}
