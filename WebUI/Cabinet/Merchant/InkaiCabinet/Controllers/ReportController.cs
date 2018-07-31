using InkaiCabinet.Models;
using InkaiCabinet.utils;
using System.Collections.Generic;
using System.Web.Mvc;

namespace InkaiCabinet.Controllers
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
