using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace loginAppRedis.Services
{
    public class CacheHandlerService : CacheService.CacheServiceBase
    {
        public override async Task<GetCacheResMessage> GetCache(GetCacheReqMessage request, ServerCallContext context)
        {
            
        }
    }
}
