using System.Web.Mvc;

namespace SupplierCabinet.Controllers
{
    public class AnalyticsController : Controller {
        [HttpGet]
        public ActionResult GeneralStatistics() {
            return View();
        }

        [HttpGet]
        public ActionResult WinningGraph() {
            return View();
        }

        [HttpGet]
        public ActionResult Reconciliation() {
            return View();
        }

        [HttpGet]
        public ActionResult Protocols()
        {
            return View();
        }
    }
}