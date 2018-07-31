using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BrokerCabinet.Controllers
{
    public class AnalyticsController : Controller
    {
        [HttpGet]
        public ActionResult AuctionsReport(int? id) {
            if(id == null) return View();
            else return View("Auction", id);
        }
    }
}