using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientCache.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        public string Index()
        {
            return "home test";
        }

    }
}
