using VostokViewCabinet.Models;
using VostokViewCabinet.utils;
using System.Collections.Generic;
using System.Web.Mvc;

namespace VostokViewCabinet.Controllers
{
    public class ReportController : Controller
    {
        DataManagerTest dataManager;

        public ReportController() {
            dataManager = new DataManagerTest();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(dataManager.GetOrders());
        }
    }
}
