using ClientCache.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientCache.Logic
{
    public class LoginLogic
    {
        public async Task<string> TryAuthenticate(LoginModel req)
        {
            return await Task.FromResult("");
        }
    }
}
