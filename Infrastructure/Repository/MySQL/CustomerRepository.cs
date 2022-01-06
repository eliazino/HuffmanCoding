using AutoMapper;
using Core.Application.DTOs.Configurations;
using Core.Application.DTOs.Filter;
using Core.Application.DTOs.Local;
using Core.Application.DTOs.Request;
using Core.Application.Interfaces.JobQueue;
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
    public class CustomerRepository : Repository, ICustomerRepository, IRepository {
        private readonly IDBCommand _DBcontext;        
        private readonly IMapper _mapper;
        private readonly SystemVariables _sysVar;
        private readonly IJobQueue _queue;
        public CustomerRepository(IDBCommand IDBCommand, IMapper _mapper, IJobQueue queue, IOptionsMonitor<SystemVariables> config) :base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
            this._sysVar = config.CurrentValue;
            this._queue = queue;
        }

        public async Task<bool> create(Customer customer) {
            string query = "INSERT INTO customer (phone, email, fullname, cardSerial, creator, gender, dob, dateAdded, issuerID, password, wemacc, pin, username, account) VALUES (@phone, @email, :fullname, @cardSerial, @creator, @gender, @dob, @dateAdded, @issuerID, @password, @wemacc, @pin, @username, @account)";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@phone", customer.phone);
            _DBcontext.bindValue("@email", customer.email);
            _DBcontext.bindValue("@fullname", customer.fullname);
            _DBcontext.bindValue("@cardSerial", customer.cardSerial);
            _DBcontext.bindValue("@creator", customer.creator);
            _DBcontext.bindValue("@gender", customer.gender);
            _DBcontext.bindValue("@dob", customer.dob);
            _DBcontext.bindValue("@dateAdded", customer.dateAdded);
            _DBcontext.bindValue("@issuerID", customer.issuerID);
            _DBcontext.bindValue("@password", customer.password);
            _DBcontext.bindValue("@wemacc", customer.wemacc);
            _DBcontext.bindValue("@pin", customer.pin);
            _DBcontext.bindValue("@username", customer.username);
            _DBcontext.bindValue("@account", customer.account);
            bool created = await _DBcontext.execute();
            if (created) {
                try {
                    var config = _queue.config(_sysVar.QueueServer.Jobs.Registration, true, null);
                    _queue.send(customer, config);
                } catch {}
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<T>> get<T>(CustomerFilter filter) where T : class {
            string query = "SELECT `id`, `fullname`, `email`, `phone`, `creator`, `cardSerial`, `gender`, `issuerID`, IF(`dp` IS NULL, 'empty', dp) as dp, `dateAdded`, `dob`, `valid`, `nok`, `nokphone`, `address`, username, IF((username IS NULL), 0, 1) as profileLoginUpdated, pin, wemacc, account FROM customer WHERE 1 = 1";
            string clause = "";
            List<object> parameters = new List<object>();
            if(filter.fieldIsSet(nameof(filter.username)) && filter.fieldIsSet(nameof(filter.password))) {
                clause += " AND (username = ? or phone = ?) AND password = ?";
                parameters.Add(filter.username);
                parameters.Add(filter.username);
                parameters.Add(filter.password);
            } else {
                if (filter.fieldIsSet(nameof(filter.username))) {
                    clause += " AND username = ?";
                    parameters.Add(filter.username);
                }
                if (filter.fieldIsSet(nameof(filter.password))) {
                    clause += " AND password = ?";
                    parameters.Add(filter.password);
                }                
            }
            if (filter.fieldIsSet(nameof(filter.phone))) {
                clause += " AND phone = ?";
                parameters.Add(filter.phone);
            }
            if (filter.fieldIsSet(nameof(filter.cardSerial))) {
                clause += " AND cardSerial = ?";
                parameters.Add(filter.cardSerial);
            }
            if (filter.fieldIsSet(nameof(filter.publicKey))) {
                clause += " AND publicKey = ?";
                parameters.Add(filter.publicKey);
            }
            if (filter.fieldIsSet(nameof(filter.wemacc))) {
                clause += " AND wemacc = ?";
                parameters.Add(filter.wemacc);
            }
            if (filter.fieldIsSet(nameof(filter.email))) {
                clause += " AND email = ?";
                parameters.Add(filter.email);
            }
            query = query + clause;
            var result = await selectFromQuery<T>(query, parameters);
            return result.resultAsObject;
        }
        public async Task<IEnumerable<Customer>> get(CustomerFilter filter) {
            return await this.get<Customer>(filter);
        }

        public async Task<bool> updateSession(string publicKey, long lastseen, string username) {
            string query = "UPDATE customer SET publicKey = @key, lastseen = @lastseen WHERE username = @username";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@key", publicKey);
            _DBcontext.bindValue("@lastseen", lastseen);
            _DBcontext.bindValue("@username", username);
            return await _DBcontext.execute();
        }

        public async Task<IEnumerable<CardBalance>> getAccounts(string phone) {
            string query = "SELECT customer.cardSerial, cardBalance as balance, orgID AS cardName FROM customer, cards WHERE cards.cardSerial = customer.cardSerial AND cards.status = 1 AND phone LIKE CONCAT('%', ?)";
            var cards = (List<CardBalance>)(await selectFromQuery<CardBalance>(query, new List<object> { phone })).resultAsObject;
            List<CardBalance> newBalance = new List<CardBalance>();
            foreach(CardBalance card in cards) {
                var balance = await this.getCardBalance(card.cardSerial);
                if(balance != null) {
                    card.balance = balance.balance;
                    card.lastUpdated = balance.lastUpdated;                    
                }
                newBalance.Add(card);
            }
            return newBalance;
        }

        private async Task<CardBalance> getCardBalance(string cardSerial) {
            string query = @"SELECT cardBalance as balance, transDate as lastUpdated, cardSerial FROM (
                (SELECT (cardBalance + famount - amount) AS cardBalance, transDate, cardSerial FROM bustransactions WHERE cardSerial = ? AND transID NOT LIKE '_walletPay%' ORDER BY transDate DESC LIMIT 1) 
                UNION (SELECT cardBalance, transDate, cardSerial FROM transactions WHERE cardSerial = ? ORDER BY transDate DESC LIMIT 1)
                UNION (SELECT finalCardBal AS cardBalance, transDate, cardSerial FROM transfers WHERE source = 'card' AND cardSerial = ? ORDER BY transDate DESC LIMIT 1)
                ) AS tmp ORDER BY tmp.transDate DESC LIMIT 1";
            var data = (List<CardBalance>)(await selectFromQuery<CardBalance>(query, new List<object> { cardSerial, cardSerial, cardSerial })).resultAsObject;
            if (data.Count < 1)
                return null;
            return data[0];
        }

        public async Task<double> getBalance(string username) {
            var result = (List<CardBalance>)(await selectFromQuery<CardBalance>("SELECT walletBalance as balance, cardSerial from customer WHERE username = ?", new List<object> { username })).resultAsObject;
            return result.Count > 0 ? result[0].balance : 0;            
        }        

        public async Task<bool> updatePassword(string username, string password) {
            string query = "UPDATE customer SET password = @password WHERE username = @username";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@password", password);
            _DBcontext.bindValue("@username", username);
            return await _DBcontext.execute();
        }

        public async Task<bool> updateSelf(Customer customer) {
            string query = "UPDATE customer SET email = @email, fullname = @fullname, dob = @dob, address = @address, dp = @dp, gender = @gender WHERE username IS NOT NULL AND username = @username";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@email", customer.email);
            _DBcontext.bindValue("@fullname", customer.fullname);            
            _DBcontext.bindValue("@dob", customer.dob);
            _DBcontext.bindValue("@username", customer.username);
            _DBcontext.bindValue("@address", customer.address);
            _DBcontext.bindValue("@gender", customer.gender);
            _DBcontext.bindValue("@dp", customer.dp);            
            await _DBcontext.execute();
            return _DBcontext.lastAffectedRows > 0;
        }

        public async Task<bool> updateBankAccount(string username, string account) {
            string query = "UPDATE customer SET wemacc = @wemacc WHERE username = @username";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@username", username);
            _DBcontext.bindValue("@wemacc", account);
            return await _DBcontext.execute();
        }

        public async Task<bool> debitWallet(double amount, string username) {
            string query = "UPDATE customer SET walletBalance = walletBalance - @amount WHERE username = @username";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@amount", amount);
            _DBcontext.bindValue("@username", username);
            return await _DBcontext.execute();
        }

        public async Task<bool> firstUpdate(Customer customer) {
            _DBcontext.prepare("UPDATE customer SET password = @password, username = @username, pin = @pin, wemacc = @wemacc WHERE phone = @phone");
            _DBcontext.bindValue("@username", customer.username);
            _DBcontext.bindValue("@email", customer.email);
            _DBcontext.bindValue("@password", customer.password);
            _DBcontext.bindValue("@pin", customer.pin);
            _DBcontext.bindValue("@phone", customer.phone);
            _DBcontext.bindValue("@wemacc", customer.wemacc);
            return await _DBcontext.execute();
        }

        public async Task<bool> customerExists(CustomerFilter filter) {
            string query = "SELECT `id` FROM customer WHERE 1 = 1";
            string clause = "";
            List<object> parameters = new List<object>();
            if (filter.fieldIsSet(nameof(filter.username)) && filter.fieldIsSet(nameof(filter.password))) {
                clause += " AND (username = ? or phone = ?) AND password = ?";
                parameters.Add(filter.username);
                parameters.Add(filter.username);
                parameters.Add(filter.password);
            } else {
                if (filter.fieldIsSet(nameof(filter.username))) {
                    clause += " AND username = ?";
                    parameters.Add(filter.username);
                }
                if (filter.fieldIsSet(nameof(filter.password))) {
                    clause += " AND password = ?";
                    parameters.Add(filter.password);
                }
            }
            if (filter.fieldIsSet(nameof(filter.phone))) {
                clause += " AND phone = ?";
                parameters.Add(filter.phone);
            }
            if (filter.fieldIsSet(nameof(filter.cardSerial))) {
                clause += " AND cardSerial = ?";
                parameters.Add(filter.cardSerial);
            }
            if (filter.fieldIsSet(nameof(filter.publicKey))) {
                clause += " AND publicKey = ?";
                parameters.Add(filter.publicKey);
            }
            if (filter.fieldIsSet(nameof(filter.wemacc))) {
                clause += " AND wemacc = ?";
                parameters.Add(filter.wemacc);
            }
            if (filter.fieldIsSet(nameof(filter.email))) {
                clause += " AND email = ?";
                parameters.Add(filter.email);
            }
            query = query + clause;
            return await isExist(query, parameters);
        }

        public async Task<bool> updatePIN(string pin, string username) {
            string query = "UPDATE customer SET pin = @pin WHERE username = @username";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@pin", pin);
            _DBcontext.bindValue("@username", username);
            return await _DBcontext.execute();
        }

        public async Task<bool> updateProfileImage(string username, string path) {
            string query = "UPDATE customer SET dp = @dp WHERE username = @username";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@dp", path);
            _DBcontext.bindValue("@username", username);
            return await _DBcontext.execute();
        }
    }
    
}
