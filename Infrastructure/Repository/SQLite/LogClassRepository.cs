using Core.Application.Interfaces.Repository;
using Core.Models.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.SQlite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.SQLite {
    public class LogClassRepository : Repository, IRepository, ILogClassRepository {
        private readonly IDBCommand _DBcontext;
        private readonly AutoMapper.IMapper _mapper;
        public LogClassRepository(IDBCommand IDBCommand, AutoMapper.IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
            _ = _init();
        }

        public async Task<bool> create(LogClass data) {
            string sql = "INSERT INTO LogClass (name) VALUES (:name)";
            _DBcontext.prepare(sql);
            _DBcontext.bindValue("@fullname", data.name);
            return await _DBcontext.execute();
        }

        public async Task<List<LogClass>> get(long id) {
            return (List<LogClass>)((await selectFromQuery<LogClass>("select * from LogClass WHERE id = ?", new List<object> { id })).resultAsObject);
        }

        public async Task<List<LogClass>> get() {
            return (List<LogClass>)((await selectFromQuery<LogClass>("select * from LogClass", new List<object> { })).resultAsObject);
        }

        public Task<bool> update(LogClass data) {
            throw new NotImplementedException();
        }

        private async Task<bool> _init() {
            return await createTableIfNotExists<LogClass>(new LogClass() { });
        }
    }
}
