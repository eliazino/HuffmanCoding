using AutoMapper;
using Core.Application.DTOs.Filter;
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
    public class BankAccountRepository : Repository, IRepository, IBankAccountRepository {
        private readonly IDBCommand _DBcontext;
        private readonly IMapper _mapper;
        public BankAccountRepository(IDBCommand IDBCommand, IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
        }

        public async Task<bool> createAccount(BankAccount account) {
            string query = "INSERT INTO bankaccount (accountNumber, bankName, phone, username, fullname, prefered) VALUES (@accountNumber, @bankName, @phone, @username, @fullname, @prefered)";
            _DBcontext.prepare(query);
            _DBcontext.bindValue("@accountNumber", account.accountNumber);
            _DBcontext.bindValue("@bankName", account.bankName);
            _DBcontext.bindValue("@phone", account.phone);
            _DBcontext.bindValue("@username", account.username);
            _DBcontext.bindValue("@fullname", account.fullname);
            _DBcontext.bindValue("@prefered", account.prefered);
            return await _DBcontext.execute();
        }

        public async Task<List<BankAccount>> getAccount(BankAccountFilter filter) {
            string clause = string.Empty;
            List<object> param = new List<object>();
            if (filter.fieldIsSet(nameof(filter.account))){
                clause += " AND accountNumber = ?";
                param.Add(filter.account);
            }
            if (filter.fieldIsSet(nameof(filter.phone))) {
                clause += " AND phone = ?";
                param.Add(filter.phone);
            }
            if (filter.fieldIsSet(nameof(filter.username))) {
                clause += " AND username = ?";
                param.Add(filter.username);
            }
            var result = (List<BankAccount>)(await selectFromQuery<BankAccount>("SELECT * from bankaccount WHERE id > 0 "+clause+" ORDER BY id DESC", param)).resultAsObject;
            return result;
        }
    }
}
