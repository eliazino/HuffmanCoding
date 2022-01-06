using AutoMapper;
using Core.Application.DTOs.Local;
using Core.Application.Interfaces.Repository;
using Core.Models.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.MySQL;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.MySQL {
    [RegisterAsScoped]
    public class TransactionsRepository : Repository, ITransactionsRepository, IRepository {
        private readonly IDBCommand _DBcontext;
        private readonly IMapper _mapper;
        public TransactionsRepository(IDBCommand IDBCommand, IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
        }

        public async Task<bool> walletPayment(WalletTransfer transferData, bool debitWallet = true) {
            _DBcontext.beginTransaction();
            string logTransfer = "insert into transfers (phone, cardSerial, agentID, cashierID, issuerID, amount, source, transDate, formerWalletBal, formerCardBal, transID, finalWalletBal, finalCardBal, beneficiary) select :phone, :cardSerial, :agentID, :cashierID, :issuerID, :amount, :source, :transDate, walletBalance, cardBalance, :transID, walletBalance - :amount, cardBalance, :beneficiary from customer where phone = :phone";
            string transferFrom = "update customer set walletBalance = walletBalance - :amount where phone = :phone";
            _DBcontext.prepare(logTransfer);
            _DBcontext.bindValue("@phone", transferData.phone);
            _DBcontext.bindValue("@cardSerial", transferData.cardSerial);
            _DBcontext.bindValue("@agentID", transferData.agentID);
            _DBcontext.bindValue("@cashierID", transferData.cashierID);
            _DBcontext.bindValue("@issuerID", transferData.issuerID);
            _DBcontext.bindValue("@amount", transferData.amount);
            _DBcontext.bindValue("@source", transferData.source);
            _DBcontext.bindValue("@transDate", transferData.transDate);
            _DBcontext.bindValue("@transID", transferData.transID);
            _DBcontext.bindValue("@beneficiary", transferData.beneficiary);
            await _DBcontext.execute();
            if (debitWallet) {
                _DBcontext.prepare(transferFrom);
                _DBcontext.bindValue("@amount", transferData.amount);
                _DBcontext.bindValue("@phone", transferData.phone);
                await _DBcontext.execute();
            }
            return await _DBcontext.commit();
        }
        public async Task<bool> refundTicket(string transID, string phone, string creditor, double amount) {
            _DBcontext.beginTransaction();
            string transferFrom = "update customer set walletBalance = walletBalance + :amount where phone = :phone";
            string updateTransfer = "update transfers set issuerID = :creditor where transID = :transID";            
            _DBcontext.prepare(transferFrom);
            _DBcontext.bindValue("@phone", phone);
            _DBcontext.bindValue("@amount", amount);
            await _DBcontext.execute();
            _DBcontext.prepare(updateTransfer);
            _DBcontext.bindValue("@transID", transID);
            _DBcontext.bindValue("@creditor", creditor);
            await _DBcontext.execute();
            return await _DBcontext.commit();
        }        
        public async Task<bool> walletTransfer(WalletTransfer transferData) {
            _DBcontext.beginTransaction();
            string logTransfer = "INSERT INTO transfers (phone, cardSerial, agentID, cashierID, issuerID, amount, source, transDate, formerWalletBal, formerCardBal, transID, finalWalletBal, finalCardBal, beneficiary) select :phone, cardSerial, :agentID, :cashierID, :issuerID, :amount, :source, :transDate, walletBalance, cardBalance, :transID, walletBalance - :amount, cardBalance, :beneficiary from customer where phone = :phone";
            string transferFrom = "update customer set walletBalance = walletBalance - :amount where phone = :phone";
            string transferTo = "update customer set walletBalance = walletBalance + :amount where phone = :phone";            
            _DBcontext.prepare(logTransfer);
            _DBcontext.bindValue("@phone", transferData.phone);
            _DBcontext.bindValue("@agentID", transferData.agentID);
            _DBcontext.bindValue("@cashierID", transferData.cashierID);
            _DBcontext.bindValue("@issuerID", transferData.issuerID);
            _DBcontext.bindValue("@amount", transferData.amount);
            _DBcontext.bindValue("@source", transferData.source);
            _DBcontext.bindValue("@transDate", transferData.transDate);
            _DBcontext.bindValue("@transID", transferData.transID);
            _DBcontext.bindValue("@beneficiary", transferData.beneficiary);
            await _DBcontext.execute();
            _DBcontext.prepare(transferFrom);
            _DBcontext.bindValue("@amount", transferData.amount);
            _DBcontext.bindValue("@phone", transferData.phone);
            await _DBcontext.execute();
            _DBcontext.prepare(transferTo);
            _DBcontext.bindValue("@amount", transferData.amount);
            _DBcontext.bindValue("@phone", transferData.beneficiary);
            await _DBcontext.execute();
            return await _DBcontext.commit();
        }
        public async Task<bool> creditWallet(WalletTopupDTO topupData) {
            _DBcontext.beginTransaction();
            _DBcontext.prepare("INSERT INTO `topup` (agentID, cashierID, transDate, admin, amount, transID, issuerID, formerbalance, type) SELECT customer.cardSerial, customer.username, UNIX_TIMESTAMP(), :admin, :amount, :reference, CONCAT(customer.issuerID, '_'), customer.walletBalance, :type from customer where phone = :phone");
            _DBcontext.bindValue("@admin", topupData.source);
            _DBcontext.bindValue("@amount", topupData.amount);
            _DBcontext.bindValue("@reference", topupData.transID);
            _DBcontext.bindValue("@phone", topupData.phone);
            _DBcontext.bindValue("@type", topupData.type);
            await _DBcontext.execute();
            _DBcontext.prepare("UPDATE customer SET walletBalance = walletBalance + :amount WHERE phone = :phone");
            _DBcontext.bindValue("@amount", topupData.amount);
            _DBcontext.bindValue("@phone", topupData.phone);
            await _DBcontext.execute();
            return await _DBcontext.commit();
        }
        public async Task<bool> walletCredited(string transID) {
            string sql = "SELECT id FROM topup WHERE transID = ? UNION SELECT id FROM topup_inv WHERE transID = ?";
            return await isExist(sql, new List<object> { transID, transID });
        }
    }
}
