using AutoMapper;
using Core.Application.Interfaces.Repository.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using NetCore.AutoRegisterDi;
using StackExchange.Redis.Extensions.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence {
    [RegisterAsSingleton]
    public class RedisCache : ICacheService {
        private readonly IRedisCacheClient _redisCacheClient;
        private readonly IMapper _mapper;
        public RedisCache(IRedisCacheClient redisCacheClient, IMapper _mapper) {
            this._redisCacheClient = redisCacheClient;
            this._mapper = _mapper;
        }
        public async Task<bool> addWithKey(string key, string value, int expiry = 600) {
            try {
                int slidingExpiry = expiry;
                return await _redisCacheClient.GetDbFromConfiguration().AddAsync<string>(key, value, DateTimeOffset.Now.AddMinutes(slidingExpiry));
            } catch {
                return false;
            }
        }

        public async Task<bool> addWithKey<T>(string key, T value, int expiry = 600) {
            try {
                int slidingExpiry = expiry;
                return await _redisCacheClient.GetDbFromConfiguration().AddAsync<T>(key, value, DateTimeOffset.Now.AddMinutes(slidingExpiry));
            } catch {
                return false;
            }
        }

        public async Task<bool> deleteWithKey(string key) {
            try {
                return await _redisCacheClient.GetDbFromConfiguration().RemoveAsync(key);
            } catch {
                return false;
            }
        }

        public async Task<T> getWithKey<T>(string key) {
            try {
                var data = await _redisCacheClient.GetDbFromConfiguration().GetAsync<T>(key);
                return data;
            } catch {
                return default(T);
            }
        }
    }
}
