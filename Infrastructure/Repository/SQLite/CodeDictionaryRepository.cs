using Core.Application.Interfaces.Repository;
using Core.Models.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.SQlite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.SQLite {
    public class CodeDictionaryRepository : Repository, IRepository, ICodeDictionaryRepository {
        private readonly IDBCommand _DBcontext;
        private readonly AutoMapper.IMapper _mapper;
        public CodeDictionaryRepository(IDBCommand IDBCommand, AutoMapper.IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
            _ = _init();
        }

        public async Task<bool> create(CodeDictionary dictionary) {
            string sql = "INSERT INTO CodeDictionary (dictionary, humanID) VALUES (:dictionary, :humanID)";
            _DBcontext.prepare(sql);
            _DBcontext.bindValue("@dictionary", dictionary.dictionary);
            _DBcontext.bindValue("@humanID", dictionary.humanID);
            return await _DBcontext.execute();
        }

        public async Task<List<CodeDictionary>> get(long id, long humanID) {
            if(id > 0) {
                return (List<CodeDictionary>)((await selectFromQuery<CodeDictionary>("select * from CodeDictionary WHERE id = ?", new List<object> { id })).resultAsObject);
            }
            return (List<CodeDictionary>)((await selectFromQuery<CodeDictionary>("select * from CodeDictionary WHERE humanID = ?", new List<object> { humanID })).resultAsObject);
        }

        public async Task<List<CodeDictionary>> get() {
            return (List<CodeDictionary>)((await selectFromQuery<CodeDictionary>("select * from CodeDictionary", new List<object> { })).resultAsObject);
        }

        public async Task<bool> update(CodeDictionary data) {
            string sql = "UPDATE CodeDictionary SET dictionary = :dictionary WHERE id = :id";
            _DBcontext.prepare(sql);
            _DBcontext.bindValue("@dictionary", data.dictionary);
            _DBcontext.bindValue("@id", data.id);
            return await _DBcontext.execute();
        }

        private async Task<bool> _init() {
            return await createTableIfNotExists<CodeDictionary>(new CodeDictionary() { });
        }
    }
}
