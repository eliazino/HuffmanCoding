using Core.Application.DTOs.Configurations;
using Core.Application.Interfaces.Repository;
using Core.Application.Interfaces.Repository.Cache;
using Core.Models.Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Others {
    public class DayPassRepository : IDayPassRepository {
        private readonly ICacheService _cacheService;
        private readonly SystemVariables _sysVar;
        public DayPassRepository(ICacheService cacheService, IOptionsMonitor<SystemVariables> config) {
            _cacheService = cacheService;
            _sysVar = config.CurrentValue;
        }

        public async Task<DayPass> getPassTransaction(string transID) {
            return await _cacheService.getWithKey<DayPass>(transID);
        }

        public async Task<DayPass> fetchPass(long id) {
            var data = await fetchPass();
            var result = data.Find(F => F.id == id);
            return result;
        }

        public async Task<bool> savePassTransaction(string transID, DayPass pass, int expiry) {
            return await _cacheService.addWithKey<DayPass>(transID, pass, expiry);
        }

        public async Task<List<DayPass>> fetchPass() {
            string fileName = _sysVar.DayPassConfig.dataFile;
            string json = await System.IO.File.ReadAllTextAsync(fileName);
            List<DayPass> data = JArray.Parse(json).ToObject<List<DayPass>>();
            return data;
        }
    }
}
