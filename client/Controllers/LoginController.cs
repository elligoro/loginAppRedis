using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientCache.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientCache.Controllers
{
    [Route("/login")]
    public class LoginController : Controller
    {
        private readonly LoginLogic _loginLogic;
        public LoginController(LoginLogic loginLogic)
        {
            _loginLogic = loginLogic;
        }
        
        [HttpGet]
        public string Index()
        {

            return "test";
        }

        [HttpPost]
        public async Task<ActionResult<string>> TryAuthenticate([FromBody] LoginModel req)
        {
            return await _loginLogic.TryAuthenticate(req);
        }
    }

    public class LoginModel
    {
        string userName { get; set; }
        string password { get; set; }
    }
}
