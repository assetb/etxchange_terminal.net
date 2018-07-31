using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SupplierCabinetDemo.Controllers
{
    public class AuthorizationController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}