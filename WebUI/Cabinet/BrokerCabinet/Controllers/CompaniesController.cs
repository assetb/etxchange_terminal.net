using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrokerCabinet.Controllers
{
    public class CompaniesController : Controller
    {
        [HttpGet]
        public ActionResult Index(int? id) {
            return id != null ? View("Supplier", id) : View();
        }
    }
}