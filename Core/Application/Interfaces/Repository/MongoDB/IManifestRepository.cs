using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Repository.MongoDB {
    public interface IManifestRepository {
        Task<bool> createManifest(Manifest manifest);
        Task<bool> updateManifest(Manifest mannifest);
        Task<List<Manifest>> getManifest(string transID);
    }
}
