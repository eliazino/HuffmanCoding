using Core.Application.Interfaces.Repository.MongoDB;
using Core.Models.Entities;
using Infrastructure.Persistence;
using NetCore.AutoRegisterDi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.MongoDB {
    [RegisterAsScoped]
    public class ManifestRepository : Repository<Manifest>, IManifestRepository, Interfaces.MongoDB.IRepository<Manifest> {
        public ManifestRepository(IMongoDb db) : base(db) { }

        public async Task<bool> createManifest(Manifest manifest) {
            return await create(manifest);
        }

        public async Task<List<Manifest>> getManifest(string transID) {
            Expression<Func<Manifest, bool>> condition = T => T.transID == transID;
            return (List<Manifest>)await getByCondition(condition);
        }

        public async Task<bool> updateManifest(Manifest manifest) {
            Expression<Func<Manifest, bool>> condition = T => T.transID == manifest.transID;
            var updateObject = new { fullname = manifest.fullname, departure = manifest.departure, gender = manifest.gender, nextOfKin = manifest.nextOfKin, phone = manifest.phone };
            return await update(updateObject, condition);
        }
    }

    [RegisterAsScoped]
    public class EscrowRepository : Repository<Transaction>, IEscrowRepository, Interfaces.MongoDB.IRepository<Transaction> {
        public EscrowRepository(IMongoDb db) : base(db) { }

        public async Task<bool> createEscrow(Transaction trx) {
            return await create(trx);
        }

        public async Task<bool> deleteEscrow(string transID) {
            Expression<Func<Transaction, bool>> condition = T => T.transID == transID;
            return await delete(condition);
        }

        public async Task<List<Transaction>> getEscrow(string transID) {
            Expression<Func<Transaction, bool>> condition = T => T.transID == transID;
            return (List<Transaction>)await getByCondition(condition);
        }
    }
}
