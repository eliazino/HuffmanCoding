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
    public class LogRepository : Repository, IRepository, ILogRepository {
        private readonly IDBCommand _DBcontext;
        private readonly AutoMapper.IMapper _mapper;
        public LogRepository(IDBCommand IDBCommand, AutoMapper.IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
            _ = _init();
        }

        public async Task<bool> create(Logs log) {
            string sql = "INSERT INTO Logs (details, humanID, logClass, logTime) VALUES (:details, :humanID, :logClass, :logTime)";
            _DBcontext.prepare(sql);
            _DBcontext.bindValue("@details", log.details);
            _DBcontext.bindValue("@humanID", log.humanID);
            _DBcontext.bindValue("@logClass", log.logClass);
            _DBcontext.bindValue("@logTime", log.logTime);
            return await _DBcontext.execute();
        }

        public async Task<List<Logs>> get(LogFilter filter) {
            string clause = "";
            string sql = "SELECT Logs.*, Human.fullname, LogClass.name FROM Logs, Human, LogClass WHERE Logs.humanID = Human.id AND Logs.logClass = LogClass.id ";
            List<object> parameters = new List<object>();
            if (filter.fieldIsSet(nameof(filter.category))) {
                clause += " AND Logs.logClass = ?";
                parameters.Add(filter.category);
            }
            if (filter.fieldIsSet(nameof(filter.humanID))) {
                clause += " AND Logs.humanID = ?";
                parameters.Add(filter.humanID);
            }
            if (filter.fieldIsSet(nameof(filter.id))) {
                clause += " AND Logs.id = ?";
                parameters.Add(filter.id);
            }
            return (List<Logs>)((await selectFromQuery<Logs>(string.Concat(sql, clause), parameters)).resultAsObject);
        }

        public Task<bool> update(Logs data) {
            throw new NotImplementedException();
        }

        private async Task<bool> _init() {
            return await createTableIfNotExists<Logs>(new Logs() { });
        }
    }
}
