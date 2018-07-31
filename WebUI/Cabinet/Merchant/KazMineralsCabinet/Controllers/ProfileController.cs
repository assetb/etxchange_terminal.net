using KazMineralsCabinet.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace KazMineralsCabinet.Controllers
{
    public class ProfileController : Controller
    {
        public ActionResult Index()
        {
            return View(1);
        }

        [HttpGet]
        public ActionResult Settings() {
            return View();
        }

        
    }
}
