using KazMineralsCabinet.Models;
using KazMineralsCabinet.utils;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KazMineralsCabinet.Controllers
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
