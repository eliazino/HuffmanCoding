using AutoMapper;
using Core.Application.DTOs.Configurations;
using Core.Application.DTOs.Filter;
using Core.Application.DTOs.Local;
using Core.Application.DTOs.Request;
using Core.Application.Interfaces.JobQueue;
using Core.Application.Interfaces.Repository;
using Core.Models.Entities;
using Core.Models.Enums;
using Core.Models.ValueObjects;
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
    public class MerchantPayRepository : Repository, IMerchantPayRepository, IRepository {
        private readonly IDBCommand _DBcontext;
        private readonly IMapper _mapper;
        private readonly SystemVariables _sysVar;
        private readonly IJobQueue _queue;
        public MerchantPayRepository(IDBCommand IDBCommand, IMapper _mapper, IJobQueue queue, IOptionsMonitor<SystemVariables> config) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
            this._sysVar = config.CurrentValue;
            this._queue = queue;
        }

        public async Task<bool> completePayment(MerchantPay transactions, CustomerDTO identity) {
            _DBcontext.beginTransaction();
            string logTransfer = "insert into transfers (phone, cardSerial, agentID, cashierID, issuerID, amount, source, transDate, formerWalletBal, formerCardBal, transID, finalWalletBal, finalCardBal, beneficiary, fee) select :phone, :cardSerial, :agentID, :cashierID, :issuerID, :amount, :source, :transDate, walletBalance, cardBalance, :transID, walletBalance - :amount, cardBalance, :beneficiary, :fee from customer where phone = :phone";
            string transferFrom = "update customer set walletBalance = walletBalance - :amount where phone = :phone";
            _DBcontext.prepare(logTransfer);
            _DBcontext.bindValue("@phone", identity.phone);
            _DBcontext.bindValue("@cardSerial", identity.cardSerial);
            _DBcontext.bindValue("@agentID", transactions.merchantID);
            _DBcontext.bindValue("@cashierID", transactions.cashierID);
            _DBcontext.bindValue("@issuerID", transactions.issuerID);
            _DBcontext.bindValue("@amount", transactions.getTotalAmount());
            _DBcontext.bindValue("@source", "Transfer");
            _DBcontext.bindValue("@transDate", transactions.transDate);
            _DBcontext.bindValue("@transID", transactions.transID);
            _DBcontext.bindValue("@beneficiary", transactions.beneficiary);
            _DBcontext.bindValue("@fee", transactions.getTotalCharge());
            await _DBcontext.execute();
            _DBcontext.prepare(transferFrom);
            _DBcontext.bindValue("@amount", transactions.getTotalAmount());
            _DBcontext.bindValue("@phone", identity.phone);
            await _DBcontext.execute();
            List<TransactionDTO> trx = new List<TransactionDTO>();
            foreach (PayItem item in transactions.Item) {
                _DBcontext.prepare("insert into transactions (transBatch, cardSerial, amount, syncDate, transDate, transID,  cashierID, merchantID, cardBalance, issuerID, item, itemID, topupfee, transType, fee, status, itemQuantity) values (:transBatch, :cardSerial, :amount, :syncDate, :transDate, :transID, :cashierID, :merchantID, :cardBalance, :issuerID, :item, :itemID, :topupfee, :transType, :fee, 1, :itemQuantity)");
                _DBcontext.bindValue("@transBatch", transactions.transID);
                _DBcontext.bindValue("@cardSerial", identity.phone);
                _DBcontext.bindValue("@amount", item.amount);
                _DBcontext.bindValue("@syncDate", transactions.transDate);
                _DBcontext.bindValue("@transDate", transactions.transDate);
                _DBcontext.bindValue("@transID", item.transID);
                _DBcontext.bindValue("@cashierID", transactions.cashierID);
                _DBcontext.bindValue("@merchantID", transactions.merchantID);
                _DBcontext.bindValue("@cardBalance", 0);
                _DBcontext.bindValue("@issuerID", transactions.issuerID);
                _DBcontext.bindValue("@item", item.itemName);
                _DBcontext.bindValue("@itemID", item.itemID);
                _DBcontext.bindValue("@topupfee", 0);
                _DBcontext.bindValue("@transType", (int)TransactionType.PAYMENT);
                _DBcontext.bindValue("@fee", item.charge);
                _DBcontext.bindValue("@itemQuantity", item.quantity);                
                await _DBcontext.execute();
                int itemID;
                if (!(int.TryParse(item.itemID, out itemID))) {
                    itemID = 0;
                }
                var trnx = new TransactionDTO { transBatch = transactions.transID, cardSerial = identity.phone, amount = item.amount, syncDate = transactions.transDate, transDate = transactions.transDate, transID = item.transID, cashierID = transactions.cashierID, merchantID = transactions.merchantID, cardBalance = 0, issuerID = transactions.issuerID, item = item.itemName, itemID = itemID, topupfee = 0, transType = (int)TransactionType.PAYMENT, fee = item.charge, itemQuantity = item.quantity };
                trx.Add(trnx);
            }
            bool completed =  await _DBcontext.commit();
            if (completed) {
                try {
                    var config = _queue.config(_sysVar.QueueServer.Jobs.Transaction, true, null);
                    foreach (TransactionDTO data in trx) {
                        _queue.send(data, config);
                    }
                } catch { }
                return true;
            }
            return false;
        }
        public async Task<bool> escrowPayment(MerchantPay transactions, CustomerDTO identity) {
            _DBcontext.beginTransaction();
            string logTransfer = "insert into transfers (phone, cardSerial, agentID, cashierID, issuerID, amount, source, transDate, formerWalletBal, formerCardBal, transID, finalWalletBal, finalCardBal, beneficiary, fee, itemName, narration) select :phone, :cardSerial, :agentID, :cashierID, :issuerID, :amount, :source, :transDate, walletBalance, cardBalance, :transID, walletBalance - :amount, cardBalance, :beneficiary, :fee, :itemName, :narration from customer where phone = :phone";
            string transferFrom = "update customer set walletBalance = walletBalance - :amount where phone = :phone";
            _DBcontext.prepare(logTransfer);
            _DBcontext.bindValue("@phone", identity.phone);
            _DBcontext.bindValue("@cardSerial", identity.cardSerial);
            _DBcontext.bindValue("@agentID", transactions.merchantID);
            _DBcontext.bindValue("@cashierID", transactions.cashierID);
            _DBcontext.bindValue("@issuerID", transactions.issuerID);
            _DBcontext.bindValue("@amount", transactions.getTotalAmount());
            _DBcontext.bindValue("@source", "Transfer");
            _DBcontext.bindValue("@transDate", transactions.transDate);
            _DBcontext.bindValue("@transID", transactions.transID);
            _DBcontext.bindValue("@beneficiary", transactions.beneficiary);
            _DBcontext.bindValue("@fee", transactions.getTotalCharge());
            _DBcontext.bindValue("@itemName", transactions.Item[0].itemName);
            _DBcontext.bindValue("@narration", transactions.Item[0].departmentID);
            await _DBcontext.execute();
            _DBcontext.prepare(transferFrom);
            _DBcontext.bindValue("@amount", transactions.getTotalAmount());
            _DBcontext.bindValue("@phone", identity.phone);
            await _DBcontext.execute();
            return await _DBcontext.commit();
        }
        public async Task<List<Merchant>> getMerchant(MerchantFilter filter) {
            string clause = "";
            List<object> param = new List<object>();
            string sql = "SELECT * FROM merchants WHERE id > 0";
            if (filter.fieldIsSet(nameof(filter.manualCharge))) {
                clause += " AND manualCharge = ?";
                param.Add(filter.manualCharge);
            }
            if (filter.fieldIsSet(nameof(filter.merchantID))) {
                clause += " AND merchantID = ?";
                param.Add(filter.merchantID);
            }
            if (filter.fieldIsSet(nameof(filter.state))) {
                clause += " AND state = ?";
                param.Add(filter.state);
            }
            if (filter.fieldIsSet(nameof(filter.id))) {
                clause += " AND id = ?";
                param.Add(filter.id);
            }
            return (List<Merchant>)(await selectFromQuery<Merchant>(sql + clause, param)).resultAsObject;
        }
        public async Task<List<Department>> getDepartment(DepartmentFilter filter) {
            string clause = "";
            List<object> param = new List<object>();
            string sql = "SELECT department.*, merchants.name as merchantName, merchants.chargeIsFlat, merchants.chargeValue, merchants.state FROM department, merchants WHERE merchants.merchantID = department.merchantID AND department.isPublic = 1 AND department.description = 'App Feature'";
            if (filter.fieldIsSet(nameof(filter.description))) {
                clause += " AND department.description = ?";
                param.Add(filter.description);
            }
            if (filter.fieldIsSet(nameof(filter.merchantID))) {
                clause += " AND department.merchantID = ?";
                param.Add(filter.merchantID);
            }
            if (filter.fieldIsSet(nameof(filter.state))) {
                clause += " AND merchants.state = ?";
                param.Add(filter.state);
            } else {
                clause += " AND merchants.state > 0";
            }
            if (filter.fieldIsSet(nameof(filter.id))) {
                clause += " AND department.id = ?";
                param.Add(filter.id);
            }
            return (List<Department>)(await selectFromQuery<Department>(sql + clause, param)).resultAsObject;
        }
        public async Task<List<Payment>> getProduct(ProductFilter filter) {
            string clause = "";
            List<object> param = new List<object>();
            string sql = "SELECT * FROM payment WHERE id > 0";            
            if (filter.fieldIsSet(nameof(filter.departmentID))) {
                clause += " AND departmentID = ?";
                param.Add(filter.departmentID);
            }
            if (filter.fieldIsSet(nameof(filter.id))) {
                clause += " AND id = ?";
                param.Add(filter.id);
            }
            return (List<Payment>)(await selectFromQuery<Payment>(sql + clause, param)).resultAsObject;
        }
        public async Task<List<Transfer>> getMerchantPayLog(MerchantPayTransferFilter filter) {
            string clause = "";
            List<object> param = new List<object>();
            string sql = "SELECT * FROM transfers WHERE id > 0";
            if (filter.fieldIsSet(nameof(filter.beneficiary))) {
                clause += " AND transfers.beneficiary = ?";
                param.Add(filter.beneficiary);
            }
            if (filter.fieldIsSet(nameof(filter.dateFrom))) {
                clause += " AND transDate BETWEEN ? AND ?";
                param.Add(filter.dateFrom);
                param.Add(filter.dateTo);
            }
            if (filter.fieldIsSet(nameof(filter.phone))) {
                clause += " AND phone = ?";
                param.Add(filter.phone);
            }
            if (filter.fieldIsSet(nameof(filter.transID))) {
                clause += " AND transID = ?";
                param.Add(filter.transID);
            }
            return (List<Transfer>)(await selectFromQuery<Transfer>(sql + clause, param)).resultAsObject;
        }
        public async Task<bool> merchantLogExists(MerchantPayTransferFilter filter) {
            string clause = "";
            List<object> param = new List<object>();
            string sql = "SELECT id FROM transfers WHERE id > 0";
            if (filter.fieldIsSet(nameof(filter.beneficiary))) {
                clause += " AND transfers.beneficiary = ?";
                param.Add(filter.beneficiary);
            }
            if (filter.fieldIsSet(nameof(filter.dateFrom))) {
                clause += " AND transDate BETWEEN ? AND ?";
                param.Add(filter.dateFrom);
                param.Add(filter.dateTo);
            }
            if (filter.fieldIsSet(nameof(filter.phone))) {
                clause += " AND phone = ?";
                param.Add(filter.phone);
            }
            if (filter.fieldIsSet(nameof(filter.transID))) {
                clause += " AND transID = ?";
                param.Add(filter.transID);
            }
            return ((List<Transfer>)(await selectFromQuery<Transfer>(sql + clause, param)).resultAsObject).Count > 0;
        }
        public async Task<List<Transaction>> getMerchantPayTransactions(MerchantPayTransactionFilter filter) {
            string clause = "";
            List<object> param = new List<object>();
            string sql = "SELECT * FROM transactions WHERE id > 0";
            if (filter.fieldIsSet(nameof(filter.batchID))) {
                clause += " AND transactions.transBatch = ?";
                param.Add(filter.batchID);
            }
            if (filter.fieldIsSet(nameof(filter.transID))) {
                clause += " AND transID = ?";
                param.Add(filter.transID);
            }
            return (List<Transaction>)(await selectFromQuery<Transaction>(sql + clause, param)).resultAsObject;
        }
    }
}
