using Core.Application.Interfaces.Repository;
using Core.Models.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Interfaces.SQlite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.SQLite {
    public class HumanRepository : Repository, IRepository, IHumanRepository {
        private readonly IDBCommand _DBcontext;
        private readonly AutoMapper.IMapper _mapper;
        public HumanRepository(IDBCommand IDBCommand, AutoMapper.IMapper _mapper) : base(IDBCommand, _mapper) {
            this._DBcontext = IDBCommand;
            this._mapper = _mapper;
            _ = _init();
        }

        public async Task<bool> create(Human data) {
            string sql = "INSERT INTO Human (fullname, gender, email, phone, creator, dob, height) VALUES (:fullname, :gender, :email, :phone, :creator, :dob, :height)";
            _DBcontext.prepare(sql);
            _DBcontext.bindValue("@fullname", data.fullname);
            _DBcontext.bindValue("@gender", data.gender);
            _DBcontext.bindValue("@email", data.email);
            _DBcontext.bindValue("@phone", data.phone);
            _DBcontext.bindValue("@creator", data.creator);
            _DBcontext.bindValue("@dob", data.dob);
            _DBcontext.bindValue("@height", data.height);
            return await _DBcontext.execute();
        }

        public async Task<List<Human>> get(long id) {
            return (List<Human>)((await selectFromQuery<Human>("select * from Human WHERE id = ?", new List<object> { id })).resultAsObject);
        }

        public async Task<List<Human>> get() {
            return (List<Human>)((await selectFromQuery<Human>("select * from Human", new List<object> { })).resultAsObject);
        }

        public Task<bool> update(Human data) {
            throw new NotImplementedException();
        }

        private async Task<bool> _init() {
            return await createTableIfNotExists<Human>(new Human() { });
        }
    }
}
