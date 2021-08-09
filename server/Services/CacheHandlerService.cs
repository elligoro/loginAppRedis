using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace loginAppRedis.Services
{
    public class CacheHandlerService : CacheService.CacheServiceBase
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly LoginLockPolicy _cacheConfig;

        public CacheHandlerService(ILogger logger, IConfiguration config, IDistributedCache cache)
        {
            _cache = cache;
            _logger = logger;
            _config = config;
            _cacheConfig = _config.GetValue<LoginLockPolicy>("LoginLockPolicy");
        }
        public override async Task<CacheResMessage> GetCache(CacheReqMessage request, ServerCallContext context)
        {
            try
            {
                var userTryCacheRes = await _cache.GetStringAsync(request.UserName);
                var now = DateTime.UtcNow;
                return new CacheResMessage { IsLocked = IsLocked(JsonConvert.DeserializeObject<CacheModel>(userTryCacheRes), _cacheConfig, now) };
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetCache: error: {ex.Message}");
                throw;
            }
        }

        public override async Task<CacheResMessage> UpdateCache(CacheReqMessage request, ServerCallContext context)
        {
            try
            {
                var userTryCacheRes = await _cache.GetStringAsync(request.UserName);
                var now = DateTime.UtcNow;
                var model = UpdateCounter(JsonConvert.DeserializeObject<CacheModel>(userTryCacheRes), _cacheConfig, now);
                await _cache.SetStringAsync(request.UserName, JsonConvert.SerializeObject(model));
                return new CacheResMessage { IsLocked = IsLocked(JsonConvert.DeserializeObject<CacheModel>(userTryCacheRes), _cacheConfig, now) };
            }
            catch(Exception ex)
            {
                _logger.LogError($"UpdateCache: error: {ex.Message}");
                throw;
            }
        }

        private bool IsLocked(CacheModel model, LoginLockPolicy cacheConfig, DateTime now) => !(model is null) && model.LockTimeStart.HasValue
                                                                                                && model.LockTimeStart.Value.AddMinutes(cacheConfig.LockTime) > now;
        private bool IsTryTimeSpan(CacheModel model, LoginLockPolicy cacheConfig, DateTime now) => !(model is null) && model.CountTimeStart.HasValue
                                                                                                    && model.CountTimeStart.Value.AddMinutes(cacheConfig.TryTimeSpan) > now;
        protected CacheModel UpdateCounter(CacheModel model, LoginLockPolicy cacheConfig, DateTime now)
        {
            var isLocked = IsLocked(model, cacheConfig, now);
            var isTryTimeSpan = IsTryTimeSpan(model, cacheConfig, now);

            if (!isLocked) // locked
            {
                if(!isTryTimeSpan) // if not locked and time span not up to date or not exists -> make new record
                {
                    model.Count = 1;
                    model.CountTimeStart = now;
                    model.LockTimeStart = null;
                }
                else
                {
                    model.Count++;
                    if(model.Count >= cacheConfig.MaxTryLogin)
                        model.LockTimeStart = now;
                }
            }

            return model;
          }        
        }

        public class CacheModel
        {
            public int Count { get; set; }
            public DateTime? CountTimeStart { get; set; }
            public DateTime? LockTimeStart { get; set; }
        }
        public class LoginLockPolicy
        {
                public int MaxTryLogin { get; set; }
                public int LockTime { get; set; }
                public int TryTimeSpan { get; set; }
        }
    }