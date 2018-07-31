using System.Web.Mvc;
using KazakhaltynCabinet.utils;

namespace KazakhaltynCabinet.Controllers
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
