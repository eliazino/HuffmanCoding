using Core.Application.DTOs.Filter;
using Core.Application.Interfaces.Repository;
using Core.Models.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.SQlite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.SQLite {
    public class AdminRepository : Repository, IRepository, IAdminRepository {
        private readonly IDBCommand _DBcontext;
        private readonly AutoMapper.IMapper _mapper;
        public AdminRepository(IDBCommand IDBCommand, AutoMapper.IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
            _ = _init();
        }
        private async Task<bool> _init() {
            return await createTableIfNotExists<Admin>(new Admin() { });
        }
        public async Task<bool> createAdmin(Admin data) {
            string sql = "INSERT INTO Admin (fullname, gender, email, phone, username, password) VALUES (:fullname, :gender, :email, :phone, :username, :password)";
            _DBcontext.prepare(sql);
            _DBcontext.bindValue("@fullname", data.fullname);
            _DBcontext.bindValue("@gender", data.gender);
            _DBcontext.bindValue("@email", data.email);
            _DBcontext.bindValue("@phone", data.phone);
            _DBcontext.bindValue("@username", data.username);
            _DBcontext.bindValue("@password", data.password);
            return await _DBcontext.execute();
        }

        public async Task<List<Admin>> getAdmin(AdminFilter data) {
            string clause = "";
            string sql = "SELECT * FROM admin";
            List<object> parameters = new List<object>();
            if (data.fieldIsSet(nameof(data.username))) {
                clause += " AND username = ?";
                parameters.Add(data.username);
            }
            if (data.fieldIsSet(nameof(data.password))) {
                clause += " AND password = ?";
                parameters.Add(data.password);
            }
            if (data.fieldIsSet(nameof(data.id))) {
                clause += " AND id = ?";
                parameters.Add(data.id);
            }
            return (List<Admin>)(await selectFromQuery<Admin>(string.Concat(sql, clause), parameters)).resultAsObject;
        }

        public async Task<bool> updateAdmin(Admin data) {
            throw new NotImplementedException();
        }
    }
}
