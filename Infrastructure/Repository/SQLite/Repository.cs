using AutoMapper;
using Core.Application.DTOs.Local;
using Core.Shared;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.SQlite;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.SQLite {
    public class Repository : IRepository {
        private IDBCommand IDBCommand;
        private readonly IMapper _mapper;
        public Repository(IDBCommand IDBCommand, IMapper _mapper) {
            this.IDBCommand = IDBCommand;
            this._mapper = _mapper;
        }

        public void beginTransacion() {
            IDBCommand.beginTransaction();
        }
        public async Task<bool> executeInLine(string sql, IEnumerable<object> q) {
            IDBCommand.prepare(sql);
            IDBCommand.bindValues(q);
            return await IDBCommand.execute();
        }
        public async Task<bool> commit() {
            return await IDBCommand.commit();
        }

        private async Task<bool> tableExists(string tableName) {
            string sql = "SELECT name FROM sqlite_master WHERE type='table' AND name = ?";
            return await isExist(sql, new List<object> { tableName });
        }

        public async Task<bool> createTableIfNotExists<T>(T tableObject) {
            string tableName = typeof(T).ToString().Split('.')[^1];
            if (await tableExists(tableName))
                return true;
            List<string> columns = new List<string>();
            Type propType = tableObject.GetType();
            foreach (PropertyInfo info in propType.GetProperties()) {
                string lineColumn = string.Empty;
                if (info.Name == "id") {
                    lineColumn = getLineCreate(info.Name, null, true, false);
                    columns.Add(lineColumn);
                    continue;
                }
                Type infoType = info.PropertyType;
                string fieldType = "TEXT";
                if (new List<Type> { typeof(int), typeof(double), typeof(float), typeof(long), typeof(decimal) }.Contains(infoType)) {
                    fieldType = "NUMERIC";
                }
                lineColumn = getLineCreate(info.Name, fieldType, false, true);
                columns.Add(lineColumn);
                continue;
            }
            string sql = "CREATE TABLE " + tableName + " (" + string.Join(',', columns) + ")";
            IDBCommand.beginTransaction();
            IDBCommand.prepare(sql);
            await IDBCommand.execute();
            return await IDBCommand.commit();
        }

        private string getLineCreate(string columnName, string type, bool autoIncrement, bool nullable) {
            type = autoIncrement ? "INTEGER" : type;
            string key = autoIncrement ? "PRIMARY KEY" : "";
            string nullableField = autoIncrement ? "" : nullable ? "NULL" : "NOT NULL";
            string autoField = autoIncrement ? "AUTOINCREMENT" : "";
            return string.Join(' ', new List<string> { columnName, type, key, autoField, nullableField });
        }

        public async Task<bool> isExist(string sql, IEnumerable<object> q) {
            try {
                var result = (List<Dictionary<string, object>>)(await selectFromQuery(sql, (List<object>)q));
                if (result != null && result.Count > 0) {
                    return true;
                }
                return false;
            } catch (Exception er) {
                throw new Exception("Could not complete request:" + er.GetBaseException().ToString());
            }
        }
        public async Task<QueryResult<T>> selectFromQuery<T>(string sql, IEnumerable<object> q) {
            var res = new QueryResult<T>();
            var data = await this.selectFromQuery(sql, (List<object>)q);
            res.resultAsObject = _mapper.Map<List<T>>(data);
            return res;
        }

        private async Task<IEnumerable<Dictionary<string, object>>> selectFromQuery(string sql, List<object> q) {
            IDBCommand.prepare(sql);
            if (q != null) { IDBCommand.bindValues(q); }
            await IDBCommand.execute();
            var data = await IDBCommand.fetchAllAsObj();
            return data;
        }

        protected class Counter {
            public int totalCount { get; set; }
        }
    }
}
